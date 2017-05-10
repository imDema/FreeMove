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
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            SetToolTips();
        }

        private void Button_Move_Click(object sender, EventArgs e)
        {
            string source, destination;
            source = textBox_From.Text;
            destination = textBox_To.Text + "\\" + Path.GetFileName(source);

            if (CheckFolders(source, destination ))
            {
                bool success;
                //MOVING
                if(Directory.GetDirectoryRoot(source) == Directory.GetDirectoryRoot(destination))
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
                        if(result == DialogResult.Yes)
                        {
                            StartMoving(destination,source,true,"Wait, moving files back...");
                        }
                    }
                }
            }
        }

        private bool StartMoving(string source, string destination, bool doNotReplace, string ProgressMessage)
        {
            return _StartMoving( new MoveDialog(source, destination, doNotReplace, ProgressMessage) );
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

        #region PrivateMethods
        

        public static void Unauthorized(Exception ex)
        {
            MessageBox.Show("ERROR: a file could not be moved, it may be in use or you may not have the required permissions.\n\nTry running this program as administrator and/or close any program that is using the file specified in the details\n\nDETAILS: " + ex.Message, "Unauthorized Access");
        }

        private bool CheckFolders(string frompath, string topath)
        {
            bool passing = true;
            string errors = "";
            try
            {
                Path.GetFullPath(frompath);
                Path.GetFullPath(topath);
            }
            catch (Exception)
            {
                errors += "ERROR, invalid path name\n\n";
                passing = false;
            }
            string pattern = "^[A-Z]:\\\\";
            if (!Regex.IsMatch(frompath,pattern) || !Regex.IsMatch(topath,pattern))
            {
                errors += "ERROR, invalid path format\n\n";
                passing = false;
            }

            if (!Directory.Exists(frompath))
            {
                errors += "ERROR, source folder doesn't exist\n\n";
                passing = false;
            }
            if (Directory.Exists(topath))
            {
                errors += "ERROR, destination folder already contains a folder with the same name\n\n";
                passing = false;
            }
            if (!Directory.Exists(Directory.GetParent(topath).FullName))
            {
                errors += "destination folder doesn't exist\n\n";
                passing = false;
            }
            string TestFile = Path.Combine(Path.GetDirectoryName(frompath), "deleteme");
            while (File.Exists(TestFile)) TestFile += new Random().Next(0, 10).ToString();
            try
            {
                //Try creating a file to check permissions
                System.Security.AccessControl.DirectorySecurity ds = Directory.GetAccessControl(frompath);
                File.Create(TestFile).Close();
            }
            catch (UnauthorizedAccessException)
            {
                errors += "You do not have the required privileges to move the directory.\nTry running as administrator\n\n";
                passing = false;
            }
            finally
            {
                if(File.Exists(TestFile))
                    File.Delete(TestFile);
            }

            //Try to create a symbolic link to check permissions
            if(!CreateSymbolicLink(TestFile, Path.GetDirectoryName(topath), SymbolicLink.Directory))
            {
                errors += "Could not create a symbolic link.\nTry running as administrator\n\n";
                passing = false;
            }
            if (Directory.Exists(TestFile))
                Directory.Delete(TestFile);

            if (!passing)
                MessageBox.Show(errors);

            return passing;
        }

        private void Reset()
        {
            textBox_From.Text = "";
            textBox_To.Text = "";
            textBox_From.Focus();
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

        #endregion

        #region EventHandlers
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
        #endregion

        private void GitHubToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/imDema/FreeMove");
        }
    }
}
