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

namespace FreeMove
{
    public partial class MoveDialog : Form
    {
        string Source, Destination;
        bool DoNotReplace;

        public bool Result;

        public MoveDialog(string from, string to, bool doNotReplace)
        {
            InitializeComponent();
            Source = from;
            Destination = to;
            DoNotReplace = doNotReplace;
        }

        public MoveDialog(string from, string to, bool doNotReplace, string message) : this(from, to, doNotReplace) { label_Progress.Text = message; }

        public MoveDialog(string from, string to) : this(from, to, false) { }
        public MoveDialog(string from, string to, string message) : this(from, to, false, message) {  }

        protected async override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            Result = await Task.Run(() => MoveFolder(Source, Destination, DoNotReplace));
            Close();
        }

        private bool MoveFolder(string source, string destination, bool doNotReplace)
        {
            CopyFolder(source, destination, doNotReplace);
            try
            {
                Directory.Delete(source, true);
                return true;
            }
            catch (UnauthorizedAccessException ex)
            {
                MoveFolder(destination, source, true);
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
            }
            string[] folders = Directory.GetDirectories(sourceFolder);
            foreach (string folder in folders)
            {
                string name = Path.GetFileName(folder);
                string dest = Path.Combine(destFolder, name);
                CopyFolder(folder, dest, doNotReplace);
            }
        }
    }
}
