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

        private void button_Move_Click(object sender, EventArgs e)
        {
            string source, destination;
            source = textBox_From.Text;
            destination = textBox_To.Text + "\\" + Path.GetFileName(source);

            if (CheckFolders(source, destination ))
            {
                Directory.Move(source, destination);
                Process mkink = new Process();
                mkink.StartInfo.FileName = "cmd.exe";
                mkink.StartInfo.Arguments = "/c mklink /j " + source + " " + destination;
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

                MessageBox.Show(ReadLog());
            }
            else
            {
                textBox_From.Text = "";
                textBox_To.Text = "";
                textBox_From.Focus();
            }
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
            File.WriteAllText(GetTempFolder() + @"\log.log", log);
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
    }
}
