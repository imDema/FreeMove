using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;

namespace FreeMove
{
    public partial class Form1 : Form
    {
        #region Initialization
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            SetToolTips();
        }

        #endregion

        #region SymLink
        [DllImport("kernel32.dll")]
        static extern bool CreateSymbolicLink(
        string lpSymlinkFileName, string lpTargetFileName, SymbolicLink dwFlags);

        enum SymbolicLink
        {
            File = 0,
            Directory = 1
        }

        private bool MakeLink(string directory, string symlink)
        {
            return CreateSymbolicLink(symlink, directory, SymbolicLink.Directory);
        }
        #endregion

        #region Private Methods
        private bool CheckFolders(string source, string destination)
        {
            bool passing = true;
            string errors = "";
            //Check for correct file path format
            try
            {
                Path.GetFullPath(source);
                Path.GetFullPath(destination);
            }
            catch (Exception)
            {
                errors += "ERROR, invalid path name\n\n";
                passing = false;
            }
            string pattern = @"^[A-Z]:\\";
            if (!Regex.IsMatch(source,pattern) || !Regex.IsMatch(destination,pattern))
            {
                errors += "ERROR, invalid path format\n\n";
                passing = false;
            }

            //Check if the chosen directory is blacklisted
            string[] Blacklist = { @"C:\Windows", @"C:\Windows\System32", @"C:\Windows\Config", @"C:\ProgramData" };
            foreach (string item in Blacklist)
            {
                if(source == item)
                {
                    errors += $"Sorry, the \"{source}\" directory cannot be moved.\n\n";
                    passing = false;
                }
            }

            //Check for existence of directories
            if (!Directory.Exists(source))
            {
                errors += "ERROR, source folder doesn't exist\n\n";
                passing = false;
            }
            if (Directory.Exists(destination))
            {
                errors += "ERROR, destination folder already contains a folder with the same name\n\n";
                passing = false;
            }
            if (!Directory.Exists(Directory.GetParent(destination).FullName))
            {
                errors += "destination folder doesn't exist\n\n";
                passing = false;
            }

            if(passing)
            {
                //Check admin privileges
                string TestFile = Path.Combine(Path.GetDirectoryName(source), "deleteme");
                while (File.Exists(TestFile)) TestFile += new Random().Next(0, 10).ToString();
                try
                {
                    //Try creating a file to check permissions
                    System.Security.AccessControl.DirectorySecurity ds = Directory.GetAccessControl(source);
                    File.Create(TestFile).Close();
                }
                catch (UnauthorizedAccessException)
                {
                    errors += "You do not have the required privileges to move the directory.\nTry running as administrator\n\n";
                    passing = false;
                }
                finally
                {
                    if (File.Exists(TestFile))
                        File.Delete(TestFile);
                }

                //Try to create a symbolic link to check permissions
                if (!CreateSymbolicLink(TestFile, Path.GetDirectoryName(destination), SymbolicLink.Directory))
                {
                    errors += "Could not create a symbolic link.\nTry running as administrator\n\n";
                    passing = false;
                }
                if (Directory.Exists(TestFile))
                    Directory.Delete(TestFile);
            }

            //Check if there's enough free space on disk
            if (passing)
            {
                long Size = 0;
                DirectoryInfo Dest = new DirectoryInfo(source);
                foreach (FileInfo file in Dest.GetFiles("*", SearchOption.AllDirectories))
                {
                    Size += file.Length;
                }
                DriveInfo DestDrive = new DriveInfo(Path.GetPathRoot(destination));
                if (DestDrive.AvailableFreeSpace < Size)
                {
                    errors += $"There is not enough free space on the {DestDrive.Name} disk. {Size / 1000000}MB required, {DestDrive.AvailableFreeSpace / 1000000} available.\n\n";
                    passing = false;
                } 
            }


            if (!passing)
                MessageBox.Show(errors);

            return passing;
        }

        private bool StartMoving(string source, string destination, bool doNotReplace, string ProgressMessage)
        {
            return _StartMoving(new MoveDialog(source, destination, doNotReplace, ProgressMessage));
        }
        private bool StartMoving(string source, string destination, bool doNotReplace)
        {
            return _StartMoving(new MoveDialog(source, destination, doNotReplace));
        }
        private bool _StartMoving(MoveDialog mvDiag)
        {
            mvDiag.ShowDialog();
            return mvDiag.Result;
        }

        private void SetToolTips()
        {
            ToolTip Tip = new ToolTip()
            {
                ShowAlways = true,
                AutoPopDelay = 5000,
                InitialDelay = 600,
                ReshowDelay = 500
            };
            Tip.SetToolTip(this.textBox_From, "Select the folder you want to move");
            Tip.SetToolTip(this.textBox_To, "Select where you want to move the folder");
            Tip.SetToolTip(this.checkBox1, "Select whether you want to hide the shortcut which is created in the old location or not");
        }

        private void Reset()
        {
            textBox_From.Text = "";
            textBox_To.Text = "";
            textBox_From.Focus();
        }

        public static void Unauthorized(Exception ex)
        {
            MessageBox.Show("ERROR: a file could not be moved, it may be in use or you may not have the required permissions.\n\nTry running this program as administrator and/or close any program that is using the file specified in the details\n\nDETAILS: " + ex.Message, "Unauthorized Access");
        }
        #endregion

        #region Event Handlers
        private void Button_Move_Click(object sender, EventArgs e)
        {
            string source, destination;
            source = textBox_From.Text.TrimEnd('\\');
            destination = Path.Combine(textBox_To.Text.TrimEnd('\\'), Path.GetFileName(source));

            if (CheckFolders(source, destination))
            {
                bool success;
                //MOVING
                if (Directory.GetDirectoryRoot(source) == Directory.GetDirectoryRoot(destination))
                {
                    try
                    {
                        Directory.Move(source, destination);
                        success = true;
                    }
                    catch (IOException ex)
                    {
                        Unauthorized(ex);
                        success = false;
                    }
                }
                else
                {
                    success = StartMoving(source, destination, false);
                }

                //LINKING
                if (success)
                {
                    if (MakeLink(destination, source))
                    {
                        if (checkBox1.Checked)
                        {
                            DirectoryInfo olddir = new DirectoryInfo(source);
                            var attrib = File.GetAttributes(source);
                            olddir.Attributes = attrib | FileAttributes.Hidden;
                        }
                        MessageBox.Show("Done.");
                        Reset();
                    }
                    else
                    {
                        var result = MessageBox.Show("ERROR creating symbolic link.\nThe folder is in the new position but the link could not be created.\nTry running as administrator\n\nDo you want to move the files back?", "ERROR, could not create a directory junction", MessageBoxButtons.YesNo);
                        if (result == DialogResult.Yes)
                        {
                            StartMoving(destination, source, true, "Wait, moving files back...");
                        }
                    }
                }
            }
        }

        private void Button_BrowseFrom_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                textBox_From.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void Button_BrowseTo_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                textBox_To.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void Button_Close_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void GitHubToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/imDema/FreeMove");
        }
        #endregion
    }
}
