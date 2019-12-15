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
        bool sameDrive; //TODO: use System.IO.Copy if the files are on the same drive
        long fileCount;
        long fileCopied = 0;

        CancellationTokenSource cts = new CancellationTokenSource();
        public override void Cancel() => cts.Cancel();

        public override Task Run()
        {
            return Task.Run(() =>
            {
                OnStart(new EventArgs());
                fileCount = Directory.GetFiles(pathFrom, "*", SearchOption.AllDirectories).Length;
                CopyDirectory(pathFrom, pathTo, cts.Token);
                OnCompleted(new EventArgs());
            }, cts.Token);
        }

        private void CopyDirectory(string dirFrom, string dirTo, CancellationToken ct)
        {
            if (!Directory.Exists(dirTo))
                Directory.CreateDirectory(dirTo);
            string[] files = Directory.GetFiles(dirFrom);
            foreach (string file in files)
            {
                if (ct.IsCancellationRequested)
                    ct.ThrowIfCancellationRequested();

                string name = Path.GetFileName(file);
                string dest = Path.Combine(dirTo, name);
                if (!File.Exists(dest))
                    File.Copy(file, dest);
                OnProgressChanged(new ProgressChangedEventArgs((float)fileCopied++ / fileCount));
            }
            string[] folders = Directory.GetDirectories(dirFrom);
            foreach (string folder in folders)
            {
                string name = Path.GetFileName(folder);
                string dest = Path.Combine(dirTo, name);
                CopyDirectory(folder, dest, ct);
            }
        }

        public CopyOperation(string pathFrom, string pathTo)
        {
            this.pathFrom = pathFrom;
            this.pathTo = pathTo;
            sameDrive = string.Equals(Path.GetPathRoot(pathFrom), Path.GetPathRoot(pathTo), StringComparison.OrdinalIgnoreCase);
        }
    }
}
