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
using System.Text.RegularExpressions;

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
            try
            {
                if (await CheckForUpdate())
                {
                    progressBar1.Dispose();
                    label1.Font = new Font("Lucida Console", label1.Font.Size);
                    label1.Text = String.Format($"Current Version: {CurrentVersion}\nLatest Version:  {NewVersion}\n\nOpen the download page?");
                    button_Cancel.Enabled = true;
                    button_Cancel.Click += delegate { Dispose(); };

                    button_Ok.Enabled = true;
                    button_Ok.Click += delegate { System.Diagnostics.Process.Start("https://github.com/ImDema/FreeMove/releases/latest"); Dispose(); };
                }
                else
                {
                    label1.Text = "There are no updates available";
                    button_Ok.Enabled = true;
                    button_Ok.Click += delegate { Dispose(); };
                }
            }
            catch(Exception ex)
            {
                if (ex.Message == Properties.Resources.GitHubErrorMessage || ex is WebException)
                {
                    label1.Text = Properties.Resources.GitHubErrorMessage;
                    progressBar1.Dispose();
                    button_Ok.Enabled = true;
                    button_Ok.Click += delegate { Dispose(); };
                }
                else throw ex;
            }
        }

        public async Task<bool> CheckForUpdate()
        {
            HttpWebRequest Req = (HttpWebRequest)WebRequest.Create("https://api.github.com/repos/ImDema/FreeMove/releases/latest");
            Req.UserAgent = "ImDema/FreeMove Updater";
            HttpWebResponse Response = (HttpWebResponse) await Req.GetResponseAsync();
            Stream ResponseStream = Response.GetResponseStream();

            TextReader Reader = new StreamReader(ResponseStream);
            const string pattern = "\"tag_name\":\"([0-9.]{5,9})\"";
            NewVersion = Regex.Match(Reader.ReadToEnd(), pattern,RegexOptions.Multiline).Groups[1].Value;

            if (NewVersion == "") throw new Exception(Properties.Resources.GitHubErrorMessage);
            Assembly assembly = Assembly.GetExecutingAssembly();
            CurrentVersion = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location).FileVersion;
            return CurrentVersion != NewVersion;
        }
    }
}
