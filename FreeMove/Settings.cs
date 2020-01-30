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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Serialization;
using System.Windows.Forms;

namespace FreeMove
{
    [Serializable]
    public class Settings
    {
        public enum PermissionCheckLevel
        {
            None,
            Fast,
            Full
        }
        //Values
        public bool AutomaticUpdate = true;

        public PermissionCheckLevel PermissionCheck = PermissionCheckLevel.Fast;

        public static bool AutoUpdate
        {
            set
            {
                var LSett = Load();
                LSett.AutomaticUpdate = value;
                Save(LSett);
            }
            get
            {
                var LSett = Load();
                return LSett.AutomaticUpdate;
            }
        }

        public static PermissionCheckLevel PermCheck
        {
            get
            {
                var LSett = Load();
                return LSett.PermissionCheck;
            }
            set
            {
                var LSett = Load();
                LSett.PermissionCheck = value;
                Save(LSett);
            }
        }

        private static void Save(Settings set)
        {
            XmlSerializer ser = new XmlSerializer(typeof(Settings));
            Directory.CreateDirectory(Path.GetDirectoryName(GetSavePath()));
            using (FileStream fs = File.OpenWrite(GetSavePath()))
            {
                ser.Serialize(fs, set);
                fs.SetLength(fs.Position);
            }
        }

        private static Settings Load()
        {
            Settings LoadedSettings = null;

            if (File.Exists(GetSavePath()))
            {
                using (FileStream fs = File.OpenRead(GetSavePath()))
                {
                    XmlSerializer ser = new XmlSerializer(typeof(Settings));
                    LoadedSettings = ser.Deserialize(fs) as Settings;
                }
            }

            if (LoadedSettings == null)
                LoadedSettings = new Settings();
            
            return LoadedSettings;
        }


        static string GetSavePath()
        {
            return Environment.GetEnvironmentVariable("appdata") + @"\FreeMove\Settings.xml";
        }

        private Settings() { }
    }
}
