// FreeMove -- Move directories without breaking shortcuts or installations 
//    Copyright(C) 2020  Luca De Martini

//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.

//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
//    GNU General Public License for more details.

//    You should have received a copy of the GNU General Public License
//    along with this program.If not, see<http://www.gnu.org/licenses/>.

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
        bool Silent = false;

        public Updater(bool Silent)
        {
            this.Silent = Silent;
            InitializeComponent();
        }
        public Updater()
        {
            InitializeComponent();
        }

        private void Updater_Shown(object sender, EventArgs e)
        {
            if (Silent)
                ShowUpdate();
            else
                Check();
        }

        private async void Check()
        {
            try
            {
                if (await CheckGitHub())
                {
                    ShowUpdate();
                }
                else
                {
                    label1.Text = "There are no updates available";
                    progressBar1.Dispose();
                    button_Ok.Enabled = true;
                    button_Ok.Click += delegate { Dispose(); };
                }
            }
            catch (Exception ex)
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

        private void ShowUpdate()
        {
            progressBar1.Dispose();
            label1.Font = new Font("Lucida Console", label1.Font.Size);
            label1.Text = String.Format($"New version available\n\nCurrent Version: {CurrentVersion}\nLatest Version:  {NewVersion}\n\nOpen the download page?");
            button_Cancel.Enabled = true;
            button_Cancel.Click += delegate { Dispose(); };

            button_Ok.Enabled = true;
            button_Ok.Click += delegate { System.Diagnostics.Process.Start("https://github.com/ImDema/FreeMove/releases/latest"); Dispose(); };
        }

        public async Task<bool> CheckGitHub()
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            Stream ResponseStream = Silent ? GetGitHubStream() : await GetGitHubStreamAsync();

            TextReader Reader = new StreamReader(ResponseStream);
            const string pattern = "\"tag_name\":\"([0-9.]{5,9})\"";
            NewVersion = Regex.Match(Reader.ReadToEnd(), pattern,RegexOptions.Multiline).Groups[1].Value;

            if (NewVersion == "") throw new Exception(Properties.Resources.GitHubErrorMessage);
            Assembly assembly = Assembly.GetExecutingAssembly();
            CurrentVersion = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location).FileVersion;
            return CurrentVersion != NewVersion;
        }

        private async Task<Stream> GetGitHubStreamAsync()
        {
            HttpWebRequest Req = (HttpWebRequest)WebRequest.Create("https://api.github.com/repos/ImDema/FreeMove/releases/latest");
            Req.UserAgent = Properties.Resources.UserAgent;
            HttpWebResponse Response = (HttpWebResponse)await Req.GetResponseAsync();
            return Response.GetResponseStream();
        }
        private Stream GetGitHubStream()
        {
            HttpWebRequest Req = (HttpWebRequest)WebRequest.Create("https://api.github.com/repos/ImDema/FreeMove/releases/latest");
            Req.UserAgent = Properties.Resources.UserAgent;
            HttpWebResponse Response = (HttpWebResponse) Req.GetResponse();
            return Response.GetResponseStream();
        }


        public static async Task<Updater> SilentCheck()
        {
            Updater updater = new Updater(true);
            try
            {
                if (await updater.CheckGitHub())
                {
                    return updater;
                }
                else return null;
            }
            catch (WebException)
            {
                return null;
            }
        }
    }
}
