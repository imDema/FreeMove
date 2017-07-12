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
        public Updater()
        {
            InitializeComponent();
        }
        public async Task<bool> CheckForUpdate()
        {
            string RetrievedVersion = "";

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
                    RetrievedVersion = Reader.Value as string + ".0";
                    break;
                }
            }
            if (RetrievedVersion == "") throw new Exception("Could not retrieve the version information from the GitHub server");
            Assembly assembly = Assembly.GetExecutingAssembly();
            string CurrentVersion = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location).FileVersion;
            return CurrentVersion != RetrievedVersion;
        }
    }
}
