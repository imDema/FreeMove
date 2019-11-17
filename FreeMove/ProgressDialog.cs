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
        struct Config
        {
            public string Source;
            public string Destination;
            public bool DoNotReplace;
        }

        bool Copying;
        int CopyValue, CopyMax;
        Config conf;

        public bool Result;

        public MoveDialog(string source, string destination, bool doNotReplace)
        {
            conf.Source = source;
            conf.Destination = destination;
            conf.DoNotReplace = doNotReplace;
            InitializeComponent();
        }

        public MoveDialog(string src, string dst, bool noReplace, string message) : this(src, dst, noReplace) { label_Message.Text = message; }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            Start();
        }

        public void Start()
        {
            //Setup Worker
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += Worker_DoWork;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;

            //Set text
            CopyMax = Directory.GetFiles(conf.Source, "*", SearchOption.AllDirectories).Length;
            progressBar1.Invoke(new Action(() => progressBar1.Maximum = CopyMax));

            //Start worker
            worker.RunWorkerAsync();
        }

        //private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e) {  }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled || e.Error != null)
                Result = false;
            else
                Result = (bool)e.Result;
            Close();
            Dispose();
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            //Args
            e.Result = MoveFolder(conf.Source, conf.Destination, conf.DoNotReplace);
        }

        private bool MoveFolder(string source, string destination, bool doNotReplace)
        {
            //Copy directory to new location
            Copying = true;
            CopyValue = 0;
            progressBar1.Invoke(new Action(() => progressBar1.Style = ProgressBarStyle.Continuous));
            Task.Run(() => UpdateProgress());
            CopyFolder(source, destination, doNotReplace);
            Copying = false;

            //Try deleting the old files
            label_Message.Invoke(new Action(() => label_Message.Text = "Please wait..."));
            label_Progress.Invoke(new Action(() => label_Progress.Text = ""));
            progressBar1.Invoke(new Action(() => progressBar1.Style = ProgressBarStyle.Marquee));

            return TryDelete(source, destination);
        }

        private bool MoveFolder(string source, string destination, bool doNotReplace, string customText)
        {
            label_Message.Invoke(new Action(() => label_Message.Text = customText));
            return MoveFolder(source, destination, doNotReplace);
        }

        private bool TryDelete(string source, string destination)
        {
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
                    case DialogResult.Retry:
                        return MoveFolder(source, destination, true);

                    default:
                    case DialogResult.Abort:
                        MoveFolder(destination, source, true, "Moving the files back, please wait...");
                        Invoke(new Action(() => MessageBox.Show("The contents of the directory were moved back to their original position.")));
                    break;

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
                    break;
                }
                Form1.Unauthorized(ex);
                return false;
            }
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

        private void Button_Cancel_Click(object sender, EventArgs e)
        {

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
