using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FreeMove.IO
{
    class MoveOperation : IOOperation
    {
        string pathFrom;
        bool sameDrive; //TODO: use System.IO.Move if the files are on the same drive
        CancellationTokenSource cts;
        CopyOperation innerCopy;

        public MoveOperation(string pathFrom, string pathTo)
        {
            cts = new CancellationTokenSource();

            this.pathFrom = pathFrom;
            sameDrive = string.Equals(Path.GetPathRoot(pathFrom), Path.GetPathRoot(pathTo), StringComparison.OrdinalIgnoreCase);

            innerCopy = new CopyOperation(pathFrom, pathTo);
        }

        public override void Cancel()
        {
            cts.Cancel();
            innerCopy?.Cancel();
        }

        public override Task Run()
        {
            if (cts.IsCancellationRequested)
            {
                base.OnCancelled(new EventArgs());
                base.OnFinish(new EventArgs());
                return Task.FromCanceled(cts.Token);
            }

            OnStart(new EventArgs());
            innerCopy.ProgressChanged += (sender, e) => OnProgressChanged(new ProgressChangedEventArgs(e.Progress * 0.99f));
            
            Task copyTask = innerCopy.Run();
            return copyTask.ContinueWith(t =>
            {
                try
                {
                    if (!cts.IsCancellationRequested)
                    {
                        Directory.Delete(pathFrom, true);
                        OnProgressChanged(new ProgressChangedEventArgs(1.0f));
                    }
                    else
                        OnCancelled(new EventArgs());
                }
                finally
                {
                    OnFinish(new EventArgs());
                }
            });
        }
    }
}
