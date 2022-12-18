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
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace FreeMove
{
    class IOHelper
    {
        #region SymLink
        //External dll functions
        [DllImport("kernel32.dll")]
        static extern bool CreateSymbolicLink(
        string lpSymlinkFileName, string lpTargetFileName, SymbolicLink dwFlags);

        enum SymbolicLink
        {
            File = 0,
            Directory = 1
        }

        public static bool MakeLink(string directory, string symlink)
        {
            return CreateSymbolicLink(symlink, directory, SymbolicLink.Directory);
        }
        #endregion

        public static IO.MoveOperation MoveDir(string source, string destination)
        {
            return new IO.MoveOperation(source, destination);
        }
        public static void CheckDirectories(string source, string destination, bool safeMode)
        {
            List<Exception> exceptions = new List<Exception>();
            //Check for correct file path format
            try
            {
                Path.GetFullPath(source);
                Path.GetFullPath(destination);
            }
            catch (Exception e)
            {
                exceptions.Add(new Exception("Invalid path", e));
            }
            string pattern = @"^[A-Za-z]:\\{1,2}";
            if (!Regex.IsMatch(source, pattern) || !Regex.IsMatch(destination, pattern))
            {
                exceptions.Add(new Exception("Invalid path format"));
            }

            //Check if the chosen directory is blacklisted
            string[] Blacklist = { @"C:\Windows", @"C:\Windows\System32", @"C:\Windows\Config", @"C:\ProgramData" };
            foreach (string item in Blacklist)
            {
                if (source == item)
                {
                    exceptions.Add(new Exception($"The \"{source}\" directory cannot be moved."));
                }
            }

            //Check if folder is critical
            if (safeMode && (
                source == Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) ||
                source == Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86)))
            {
                exceptions.Add(new Exception($"It's recommended not to move the {source} directory, you can disable safe mode in the Settings tab to override this check"));
            }

            //Check for existence of directories
            if (!Directory.Exists(source))
                exceptions.Add(new Exception("Source folder does not exist"));
            
            if (Directory.Exists(destination))
                exceptions.Add(new Exception("Destination folder already contains a folder with the same name"));

            try
            {
                Form1 form = new Form1();
                if (!form.chkBox_createDest.Checked && !Directory.Exists(Directory.GetParent(destination).FullName))
                    exceptions.Add(new Exception("Destination folder does not exist"));
            }
            catch(Exception e)
            {
                exceptions.Add(e);
            }

            // Next checks rely on the previous so if there was any exception return
            if (exceptions.Count > 0)
                throw new AggregateException(exceptions);

            //Check admin privileges
            string TestFile = Path.Combine(Path.GetDirectoryName(source), "deleteme");
            int ti;
            for (ti = 0; File.Exists(TestFile + ti.ToString()) ; ti++); // Change name if a file with the same name already exists
            TestFile += ti.ToString();

            try
            {
                // DEPRECATED // System.Security.AccessControl.DirectorySecurity ds = Directory.GetAccessControl(source);
                //Try creating a file to check permissions
                File.Create(TestFile).Close();
            }
            catch (UnauthorizedAccessException e)
            {
                exceptions.Add(new Exception("You do not have the required privileges to move the directory.\nTry running as administrator", e));
            }
            finally
            {
                if (File.Exists(TestFile))
                    File.Delete(TestFile);
            }

            //Try creating a symbolic link to check permissions
            try
            {
                if (!CreateSymbolicLink(TestFile, Path.GetDirectoryName(destination), SymbolicLink.Directory))
                    exceptions.Add(new Exception("Could not create a symbolic link.\nTry running as administrator"));
            }
            finally
            {
                if (Directory.Exists(TestFile))
                    Directory.Delete(TestFile);
            }

            // Next checks rely on the previous so if there was any exception return
            if (exceptions.Count > 0)
                throw new AggregateException(exceptions);

            long size = 0;
            DirectoryInfo dirInf = new DirectoryInfo(source);
            foreach (FileInfo file in dirInf.GetFiles("*", SearchOption.AllDirectories))
            {
                size += file.Length;
            }
            try
            {
                DriveInfo dstDrive = new(Path.GetPathRoot(destination));
                if (dstDrive.AvailableFreeSpace < size)
                    exceptions.Add(new Exception($"There is not enough free space on the {dstDrive.Name} disk. {size / 1000000}MB required, {dstDrive.AvailableFreeSpace / 1000000} available."));
            } catch (Exception e)
            {
                exceptions.Add(e);
            }

            if (exceptions.Count > 0)
                throw new AggregateException(exceptions);

            //If set to do full check try to open for write all files
            if (Settings.PermCheck != Settings.PermissionCheckLevel.None)
            {
                var exceptionBag = new System.Collections.Concurrent.ConcurrentBag<Exception>();
                Action<string> CheckFile = (file) =>
                {
                    FileInfo fi = new FileInfo(file);
                    FileStream fs = null;
                    try
                    {
                        fs = fi.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
                    }
                    catch (Exception ex)
                    {
                        exceptionBag.Add(ex);
                    }
                    finally
                    {
                        if (fs != null)
                            fs.Dispose();
                    }
                };
                if (Settings.PermCheck == Settings.PermissionCheckLevel.Fast)
                {
                    Parallel.ForEach(Directory.GetFiles(source, "*.exe", SearchOption.AllDirectories), CheckFile);
                    Parallel.ForEach(Directory.GetFiles(source, "*.dll", SearchOption.AllDirectories), CheckFile);
                } else
                {
                    Parallel.ForEach(Directory.GetFiles(source, "*", SearchOption.AllDirectories), CheckFile);
                }

                exceptions.AddRange(exceptionBag);
            }
            if (exceptions.Count > 0)
                throw new AggregateException(exceptions);
        }
    }
}
