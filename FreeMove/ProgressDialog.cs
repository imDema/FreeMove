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
using System.Threading;

namespace FreeMove
{
    public partial class MoveDialog : Form
    {
        string Source, Destination;
        bool DoNotReplace, Copying;
        int CopyValue, CopyMax;

        public bool Result;

        public MoveDialog(string from, string to, bool doNotReplace)
        {
            InitializeComponent();
            Source = from;
            Destination = to;
            DoNotReplace = doNotReplace;
        }

        public MoveDialog(string from, string to, bool doNotReplace, string message) : this(from, to, doNotReplace) { label_Message.Text = message; }

        public MoveDialog(string from, string to) : this(from, to, false) { }
        public MoveDialog(string from, string to, string message) : this(from, to, false, message) { }

        protected async override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            Result = await Task.Run(() => MoveFolder(Source, Destination, DoNotReplace));
            Close();
        }

        private bool MoveFolder(string source, string destination, bool doNotReplace)
        {
            CopyMax = Directory.GetFiles(source, "*", SearchOption.AllDirectories).Length;
            CopyValue = 0;
            Copying = true;
            Task ProgressClock = Task.Run(() => { UpdateProgress(); } );

            progressBar1.Invoke(new Action(() => progressBar1.Style = ProgressBarStyle.Continuous));
            CopyFolder(source, destination, doNotReplace);

            label_Message.Invoke (new Action(() => label_Message.Text = "Please wait..."));
            label_Progress.Invoke (new Action(() => label_Progress.Text = ""));
            progressBar1.Invoke(new Action (()=> progressBar1.Style = ProgressBarStyle.Marquee));
            Copying = false;
            try
            {
                Directory.Delete(source, true);
                return true;
            }
            catch (UnauthorizedAccessException ex)
            {
                switch((DialogResult)Invoke(new Func<DialogResult>
                    (() => MessageBox.Show(this, $"Error: {ex.Message}\n\nHow do you want to proceed?\n\"Abort\" to revert changes\n\"Ignore\" to stop the program","Error while moving contents",  MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Error,MessageBoxDefaultButton.Button2,MessageBoxOptions.RightAlign))))
                {
                    default:
                    case DialogResult.Abort:
                        MoveFolder(destination, source, true, "Moving the files back, please wait...");
                        Invoke(new Action (() => MessageBox.Show("The contents of the directory were moved back to their original position.")));
                        Form1.Unauthorized(ex);
                        return false;

                    case DialogResult.Retry:
                        return MoveFolder(source, destination, true);

                    case DialogResult.Ignore:
                        if ((DialogResult)Invoke(new Func<DialogResult>(() => MessageBox.Show(this, "Are you sure you want to proceed?\n\nBy ignoring you will leave all the files as they are now: part of the files will already be in the new location and missing from the old one!", "", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2))) != DialogResult.Yes)
                        {
                            return MoveFolder(source, destination, true);
                        }

                        //else
                        try
                        {
                            using (TextWriter tw = new StreamWriter(File.OpenWrite(Path.Combine(source + "\\~README~FREEMOVE~ERROR.txt"))))
                            {
                                 tw.Write($"There was an error when moving the files using FreeMove on {DateTime.Now.ToString()} and you chose to ignore it.\nThe rest of the contents of this directory can be found at \"{destination}\" unless they were moved.\nNext time use \"Abort\" in case of an error to move the files back or \"Retry\" to try again.\n\nIf this text file was useful or if you would have preferred it wasn't created let me know.\n");
                            }
                        }
                        catch (Exception) { }

                        Form1.Unauthorized(ex);
                        return false;
                }
            }
        }

        private bool MoveFolder(string source, string destination, bool doNotReplace, string customText)
        {
            label_Message.Invoke(new Action (()=>label_Message.Text = customText));
            return MoveFolder(source, destination, doNotReplace);
        }

        private void CopyFolder(string sourceFolder, string destFolder, bool doNotReplace)
        {
            if (!Directory.Exists(destFolder))
                Directory.CreateDirectory(destFolder);
            string[] files = Directory.GetFiles(sourceFolder);
            foreach (string file in files)
            {
                string name = Path.GetFileName(file);
                string dest = Path.Combine(destFolder, name);
                if (!(doNotReplace && File.Exists(dest)))
                    File.Copy(file, dest);
                CopyValue++;
            }
            string[] folders = Directory.GetDirectories(sourceFolder);
            foreach (string folder in folders)
            {
                string name = Path.GetFileName(folder);
                string dest = Path.Combine(destFolder, name);
                CopyFolder(folder, dest, doNotReplace);
            }
        }

        private void UpdateProgress()
        {
            progressBar1.Invoke(new Action(() => progressBar1.Maximum = CopyMax));
            while(Copying)
            {
                label_Progress.Invoke( new Action(() =>
                {
                    label_Progress.Text = $"{CopyValue}/{CopyMax}";
                    progressBar1.Value = CopyValue;
                }));
                Thread.Sleep(100);
            }
        }
    }
}
