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

        CancellationTokenSource cts;
        public override void Cancel() => cts.Cancel();

        public override Task Run()
        {
            if (cts.IsCancellationRequested)
            {
                base.OnCancelled(new EventArgs());
                base.OnFinish(new EventArgs());
                return Task.FromCanceled(cts.Token);
            }

            base.OnStart(new EventArgs());
            return Task.Run(() =>
            {
                try
                {
                    fileCount = Directory.GetFiles(pathFrom, "*", SearchOption.AllDirectories).Length;
                    CopyDirectory(pathFrom, pathTo, cts.Token);
                } catch (OperationCanceledException)
                {
                    OnCancelled(new EventArgs());
                }
                finally
                {
                    base.OnFinish(new EventArgs());
                }
            });
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
            cts = new CancellationTokenSource();

            this.pathFrom = pathFrom;
            this.pathTo = pathTo;
            sameDrive = string.Equals(Path.GetPathRoot(pathFrom), Path.GetPathRoot(pathTo), StringComparison.OrdinalIgnoreCase);
        }
    }
}
