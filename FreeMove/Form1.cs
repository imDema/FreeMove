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
        bool safeMode = true;

        #region Initialization
        public Form1()
        {
            //Initialize UI elements
            InitializeComponent();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            SetToolTips();

            //Check whether the program is set to update on its start
            if (Settings.AutoUpdate)
            {
                //Update the menu item accordingly
                checkOnProgramStartToolStripMenuItem.Checked = true;
                //Start a background update task
                Updater updater = await Task<bool>.Run(() => Updater.SilentCheck());
                //If there is an update show the update dialog
                if (updater != null) updater.ShowDialog();
            }
            if (Settings.PermCheck)
            {
                PermissionCheckToolStripMenuItem.Checked = true;
            }
        }

        #endregion

        private void Begin()
        {
            //Get the original and the new path from the textboxes
            string source, destination;
            source = textBox_From.Text.TrimEnd('\\');
            destination = Path.Combine(textBox_To.Text.Length > 3 ? textBox_To.Text.TrimEnd('\\') : textBox_To.Text, Path.GetFileName(source));

            //Check for errors before copying
            var exceptions = IOHelper.CheckDirectories(source, destination, safeMode);
            if (exceptions.Length == 0)
            {
                //Move files
                using (ProgressDialog progressDialog = new ProgressDialog("Moving files..."))
                {
                    IO.MoveOperation moveOp = new IO.MoveOperation(source, destination);

                    moveOp.ProgressChanged += (sender, e) =>
                    {
                        progressDialog.UpdateProgress(e.Progress);
                    };
                    moveOp.Finish += (sender, e) =>
                    {
                        progressDialog.Invoke((Action)progressDialog.Close);
                    };

                    progressDialog.CancelRequested += (sender, e) =>
                    {
                        moveOp.Cancel();
                        //TODO Handle Cancellation
                    };

                    Task moveTask = moveOp.Run();
                    progressDialog.ShowDialog(this); //TODO, Check if successful
                    if (!moveTask.Wait(30000))
                        throw new TimeoutException("Timed out waiting for moveTask to end.\nIf you see this please open an issue on https://github.com/imDema/FreeMove/issues/new");
                }

                if (IOHelper.MakeLink(destination, source))
                {
                    //If told to make the link hidden
                    if (checkBox1.Checked)
                    {
                        DirectoryInfo olddir = new DirectoryInfo(source);
                        var attrib = File.GetAttributes(source);
                        olddir.Attributes = attrib | FileAttributes.Hidden;
                    }
                    MessageBox.Show("Done.");
                    //Reset(); TODO Reimplement this
                }
                else
                {
                    //Handle linking error
                    var result = MessageBox.Show(Properties.Resources.ErrorUnauthorizedLink, "ERROR, could not create a directory junction", MessageBoxButtons.YesNo);
                    if (result == DialogResult.Yes)
                    {
                        using (ProgressDialog progressDialog = new ProgressDialog("Moving back files..."))
                        {
                            IO.MoveOperation moveOp = new IO.MoveOperation(destination, source);

                            moveOp.ProgressChanged += (sender, e) =>
                            {
                                float progress = ((IO.IOOperation.ProgressChangedEventArgs)e).Progress;
                                progressDialog.UpdateProgress(progress);
                            };
                            moveOp.Finish += (sender, e) =>
                            {
                                progressDialog.Invoke((Action)progressDialog.Close);
                            };

                            progressDialog.CancelRequested += (sender, e) =>
                            {
                                moveOp.Cancel();
                                //TODO Handle Cancellation
                            };

                            Task moveTask = moveOp.Run();
                            progressDialog.ShowDialog(this); //TODO, Check if successful
                            if (!moveTask.Wait(30000))
                                throw new TimeoutException("Timed out waiting for moveTask to end.\nIf you see this please open an issue on https://github.com/imDema/FreeMove/issues/new");
                        }
                    }
                }
            }
            else
            {
                var msg = "";
                foreach (var ex in exceptions)
                {
                    msg += "- " + ex.Message + "\n";
                }
                MessageBox.Show(msg, "Error");
            }
        }

        //Configure tooltips
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
            MessageBox.Show(Properties.Resources.ErrorUnauthorizedMoveDetails + ex.Message, "Error details");
        }

        #region Event Handlers
        private void Button_Move_Click(object sender, EventArgs e)
        {
            Begin();
        }

        //Show a directory picker for the source directory
        private void Button_BrowseFrom_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                textBox_From.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        //Show a directory picker for the destination directory
        private void Button_BrowseTo_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                textBox_To.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        //Start on enter key press
        private void TextBox_To_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Begin();
            }
        }

        //Close the form
        private void Button_Close_Click(object sender, EventArgs e)
        {
            Close();
        }

        //Open GitHub page
        private void GitHubToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/imDema/FreeMove");
        }

        //Open the report an issue page on GitHub
        private void ReportAnIssueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/imDema/FreeMove/issues/new");
        }

        //Show an update dialog
        private void CheckNowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new Updater().ShowDialog();
        }

        //Set to check updates on program start
        private void CheckOnProgramStartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings.ToggleAutoUpdate();
            checkOnProgramStartToolStripMenuItem.Checked = Settings.AutoUpdate;
        }
        #endregion

        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string msg = String.Format(Properties.Resources.AboutContent, System.Diagnostics.FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).FileVersion);
            MessageBox.Show(msg, "About FreeMove");
        }

        private void FullPermissionCheckToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings.TogglePermCheck();
            PermissionCheckToolStripMenuItem.Checked = Settings.PermCheck;
        }

        private void SafeModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(Properties.Resources.DisableSafeModeMessage, "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                safeMode = false;
                safeModeToolStripMenuItem.Checked = false;
                safeModeToolStripMenuItem.Enabled = false;
            }
        }
    }
}
