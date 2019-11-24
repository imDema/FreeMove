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
        bool sameDrive;

        CancellationTokenSource cts;
        public override void Cancel()
        {
            cts.Cancel();
        }

        public override Task Run()
        {
            if (cts.IsCancellationRequested)
            {
                OnCancel?.Invoke();
                return Task.FromCanceled(cts.Token);
            }

        }

        private static void CopyDirectory(string dirFrom, string dirTo)
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
            }
            string[] folders = Directory.GetDirectories(dirFrom);
            foreach (string folder in folders)
            {
                string name = Path.GetFileName(folder);
                string dest = Path.Combine(dirTo, name);
                CopyDirectory(folder, dest, ct);
            }
        }

        public CopyOperation(string pathFrom, string pathTo, CancellationToken ct)
        {
            cts = new CancellationTokenSource();

            this.pathFrom = pathFrom;
            this.pathTo = pathTo;
            sameDrive = string.Equals(Path.GetPathRoot(pathFrom), Path.GetPathRoot(pathTo), StringComparison.OrdinalIgnoreCase);
        }
    }
}
