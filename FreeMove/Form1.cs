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

namespace FreeMove
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async void button_Move_Click(object sender, EventArgs e)
        {
            string source, destination;
            source = textBox_From.Text;
            destination = textBox_To.Text + "\\" + Path.GetFileName(source);

            if (CheckFolders(source, destination ))
            {
                //MOVING

                if(Directory.GetDirectoryRoot(source) == Directory.GetDirectoryRoot(destination))
                    Directory.Move(source, destination);
                else
                {
                    ProgressDialog pdiag = new ProgressDialog(this);
                    pdiag.Show();
                    this.Enabled = false;
                    await Task.Run(() => MoveFolder(source, destination));
                    this.Enabled = true;
                    pdiag.Close();
                    pdiag.Dispose();
                }

                //LINKING
                Process mkink = new Process();
                mkink.StartInfo.FileName = "cmd.exe";
                mkink.StartInfo.Arguments = "/c \"mklink /j " + source + " " + destination +"\"";
                mkink.StartInfo.UseShellExecute = false;
                mkink.StartInfo.RedirectStandardOutput = true;
                mkink.Start();

                string output = mkink.StandardOutput.ReadToEnd();
                mkink.WaitForExit();
                WriteLog(output);

                if(checkBox1.Checked)
                {
                    DirectoryInfo olddir = new DirectoryInfo(source);
                    var attrib = File.GetAttributes(source);
                    olddir.Attributes = attrib | FileAttributes.Hidden;
                }

                MessageBox.Show(output);
                textBox_From.Text = "";
                textBox_To.Text = "";
            }
            else
            {
                textBox_From.Text = "";
                textBox_To.Text = "";
                textBox_From.Focus();
            }
        }

        private void MoveFolder(string source, string destination)
        {
            CopyFolder(source, destination);
            //TEMPORARY FOR TESTING
            //Directory.Move(source, Path.Combine(Directory.GetParent(source).FullName, "safecopy"));

            //DEFINITIVE VERSION
            Directory.Delete(source, true);
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
                errors += "ERROR, invalid path name\n";
                passing = false;
            }
            string pattern = "^[A-Z]:\\\\";
            if (!Regex.IsMatch(frompath,pattern) || !Regex.IsMatch(topath,pattern))
            {
                errors += "ERROR, invalid path format";
                passing = false;
            }

            if (!Directory.Exists(frompath))
            {
                errors += "ERROR, source folder doesn't exist";
                passing = false;
            }
            if (Directory.Exists(topath))
            {
                errors += "ERROR, destination folder already contains a folder with the same name";
                passing = false;
            }
            if (!Directory.Exists(Directory.GetParent(topath).FullName))
            {
                errors += "destination folder doesn't exist";
                passing = false;
            }

            if (!passing)
                MessageBox.Show(errors);

            return passing;
        }

        private void CopyFolder(string sourceFolder, string destFolder)
        {
            if (!Directory.Exists(destFolder))
                Directory.CreateDirectory(destFolder);
            string[] files = Directory.GetFiles(sourceFolder);
            foreach (string file in files)
            {
                string name = Path.GetFileName(file);
                string dest = Path.Combine(destFolder, name);
                File.Copy(file, dest);
            }
            string[] folders = Directory.GetDirectories(sourceFolder);
            foreach (string folder in folders)
            {
                string name = Path.GetFileName(folder);
                string dest = Path.Combine(destFolder, name);
                CopyFolder(folder, dest);
            }
        }

        private void button_BrowseFrom_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                textBox_From.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void button_BrowseTo_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                textBox_To.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void WriteLog(string log)
        {
            File.AppendAllText(GetTempFolder() + @"\log.log", log);
        }

        private string ReadLog()
        {
            return File.ReadAllText(GetTempFolder() + @"\log.log");
        }

        private string GetTempFolder()
        {
            string dir = Environment.GetEnvironmentVariable("TEMP") + @"\FreeMove";
                Directory.CreateDirectory(dir);
            return dir;
        }

        private void button_Close_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
