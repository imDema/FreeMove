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
    public partial class ProgressDialog : Form
    {
        private bool cancellable = true;
        public bool Cancellable {
            get => cancellable;
            set
            {
                cancellable = value;
                button_Cancel.Enabled = value; // WARN: could cause exception if called when button_Cancel is not intitialized!
            }
        }

        private const int BAR_RESOLUTION = 1024;
        public string Message
        {
            set => label_Message.Text = value;
            get => label_Message.Text;
        }

        public ProgressDialog(string message)
        {
            InitializeComponent();
            
            Message = message;
            progressBar1.Maximum = BAR_RESOLUTION;
            progressBar1.Value = 0;
            label_Progress.Text = Message;
        }

        public event EventHandler CancelRequested
        {
            add => button_Cancel.Click += value;
            remove => button_Cancel.Click -= value;
        }

        public void UpdateProgress(IO.IOOperation.ProgressChangedEventArgs e)
        {
            if(IsHandleCreated)
            label_Progress?.BeginInvoke(new Action(() =>
            {
                float percentage = ((float)e.Progress / e.Max);
                label_Progress.Text = e.Progress == e.Max ? "Finishing..." : $"{e.Progress}/{e.Max}";
                // label_Progress.Text = $"{percentage*100f, 3:0.0}%";
                progressBar1.Value = (int)(percentage * BAR_RESOLUTION + 0.5f);
            }));
        }
    }
}
