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
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace FreeMove.IO
{
    class CopyOperation : IOOperation
    {
        string pathFrom;
        string pathTo;
        long fileCount;
        long fileCopied = 0;

        CancellationTokenSource cts = new CancellationTokenSource();
        public override void Cancel() => cts.Cancel();

        public override async Task Run()
        {
            OnStart(new EventArgs());

            try
            {
                fileCount = Directory.GetFiles(pathFrom, "*", SearchOption.AllDirectories).Length;
                OnProgressChanged(new ProgressChangedEventArgs(0, fileCount));
                await Task.Run(() => CopyDirectory(pathFrom, pathTo, cts.Token), cts.Token);
            }
            finally
            {
                OnEnd(new EventArgs());
            }
        }

        private void CopyDirectory(string dirFrom, string dirTo, CancellationToken ct)
        {
            if (!Directory.Exists(dirTo))
                Directory.CreateDirectory(dirTo);
            string[] files = Directory.GetFiles(dirFrom);
            foreach (string file in files)
            // Parallel.ForEach(files, file =>
            {
                if (ct.IsCancellationRequested)
                    ct.ThrowIfCancellationRequested();

                string name = Path.GetFileName(file);
                string dest = Path.Combine(dirTo, name);
                if (!File.Exists(dest))
                    File.Copy(file, dest);
                OnProgressChanged(new ProgressChangedEventArgs(++fileCopied, fileCount));
            }// );
            string[] folders = Directory.GetDirectories(dirFrom);
            foreach (string folder in folders)
            // Parallel.ForEach(folders, folder =>
            {
                string name = Path.GetFileName(folder);
                string dest = Path.Combine(dirTo, name);
                CopyDirectory(folder, dest, ct);
            }// );
        }

        public CopyOperation(string pathFrom, string pathTo)
        {
            this.pathFrom = pathFrom;
            this.pathTo = pathTo;
        }
    }
}
