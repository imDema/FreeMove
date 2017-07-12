using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;

namespace FreeMove
{
    public partial class Updater : Form
    {
        string CurrentVersion = "", NewVersion = "";

        public Updater()
        {
            InitializeComponent();
        }

        private async void Updater_Load(object sender, EventArgs e)
        {
            if(await CheckForUpdate())
            {
                progressBar1.Dispose();
                label1.Font = new Font("Lucida Console", label1.Font.Size);
                label1.Text = String.Format($"Current Version: {CurrentVersion}\nLatest Version:  {NewVersion}\n\nOpen the download page?");
                button_Cancel.Enabled = true;
                button_Cancel.Click += delegate { Dispose(); };

                button_Ok.Enabled = true;
                button_Ok.Click += delegate { System.Diagnostics.Process.Start("https://github.com/ImDema/FreeMove/releases/latest"); Dispose(); };
            }
        }

        public async Task<bool> CheckForUpdate()
        {
            HttpWebRequest Req = (HttpWebRequest)WebRequest.Create("https://api.github.com/repos/ImDema/FreeMove/releases/latest");
            Req.UserAgent = "ImDema/FreeMove Updater";
            HttpWebResponse Response = (HttpWebResponse) await Req.GetResponseAsync();
            Stream ResponseStream = Response.GetResponseStream();
            JsonTextReader Reader = new JsonTextReader(new StreamReader(ResponseStream));
            while(await Reader.ReadAsync())
            {
                if(Reader.TokenType == JsonToken.PropertyName && (string)Reader.Value == "tag_name")
                {
                    Reader.Read();
                    NewVersion = Reader.Value as string + ".0";
                    break;
                }
            }
            if (NewVersion == "") throw new Exception("Could not retrieve the version information from the GitHub server");
            Assembly assembly = Assembly.GetExecutingAssembly();
            CurrentVersion = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location).FileVersion;
            return CurrentVersion != NewVersion;
        }

        private void Button2_Click(object sender, EventArgs e)
        {

        }

        private void Button1_Click(object sender, EventArgs e)
        {

        }
    }
}
