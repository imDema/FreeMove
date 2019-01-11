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
        bool Copying;
        int CopyValue, CopyMax;

        public bool Result;

        public MoveDialog()
        {
            InitializeComponent();
        }

        public MoveDialog(string message) : this() { label_Message.Text = message; }

        public void Start(string source, string destination, bool doNotReplace)
        {
            //Setup Worker
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += Worker_DoWork;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            //worker.ProgressChanged += Worker_ProgressChanged;

            CopyMax = Directory.GetFiles(source, "*", SearchOption.AllDirectories).Length;
            progressBar1.Invoke(new Action(() => progressBar1.Maximum = CopyMax));

            worker.RunWorkerAsync(new Tuple<string, string, bool>(source, destination, doNotReplace));
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {


        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Result = (bool)e.Result;
            Close();
            Dispose();
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            //Args
            Tuple<string, string, bool> Args = e.Argument as Tuple<string, string, bool>;
            MoveFolder(Args.Item1, Args.Item2, Args.Item3);
        }

        private bool MoveFolder(string source, string destination, bool doNotReplace)
        {
            //Start progress watchdog
            Copying = true;
            Task.Run(() => UpdateProgress());
            CopyFolder(source, destination, doNotReplace);
            Copying = false;

            label_Message.Invoke(new Action(() => label_Message.Text = "Please wait..."));
            label_Progress.Invoke(new Action(() => label_Progress.Text = ""));
            progressBar1.Invoke(new Action(() => progressBar1.Style = ProgressBarStyle.Marquee));
            try
            {
                Directory.Delete(source, true);
                return true;
            }
            catch (UnauthorizedAccessException ex)
            {
                switch ((DialogResult)Invoke(new Func<DialogResult>
                    (() => MessageBox.Show(this, String.Format(Properties.Resources.ErrorUnauthorizedMoveMessage, ex.Message), "Error while moving contents\n Unauthorized Access Exception", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Error, MessageBoxDefaultButton.Button2))))
                {
                    default:
                    case DialogResult.Abort:
                        MoveFolder(destination, source, true, "Moving the files back, please wait...");
                        Invoke(new Action(() => MessageBox.Show("The contents of the directory were moved back to their original position.")));
                        return false;

                    case DialogResult.Retry:
                        return MoveFolder(source, destination, true);

                    case DialogResult.Ignore:
                        if ((DialogResult)Invoke(new Func<DialogResult>(() => MessageBox.Show(this, Properties.Resources.IgnoreMessage, "", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2))) != DialogResult.Yes)
                        {
                            return MoveFolder(source, destination, true);
                        }

                        //else
                        try
                        {
                            using (TextWriter tw = new StreamWriter(File.OpenWrite(Path.Combine(source + "\\~README~FREEMOVE~ERROR.txt"))))
                            {
                                tw.Write(String.Format(Properties.Resources.IgnoreTextFile, DateTime.Now.ToString(), destination));
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
            label_Message.Invoke(new Action(() => label_Message.Text = customText));
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
            while (Copying)
            {
                label_Progress.Invoke(new Action(() =>
                {
                    label_Progress.Text = $"{CopyValue}/{CopyMax}";
                    progressBar1.Value = CopyValue;
                }));
                Thread.Sleep(100);
            }
        }
    }
}
