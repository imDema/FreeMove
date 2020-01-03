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
        CancellationTokenSource cts = new CancellationTokenSource();
        CopyOperation innerCopy;

        public MoveOperation(string pathFrom, string pathTo)
        {
            this.pathFrom = pathFrom;
            sameDrive = string.Equals(Path.GetPathRoot(pathFrom), Path.GetPathRoot(pathTo), StringComparison.OrdinalIgnoreCase);

            innerCopy = new CopyOperation(pathFrom, pathTo);
        }

        public override void Cancel()
        {
            cts.Cancel();
            innerCopy?.Cancel();
        }

        public override async Task Run()
        {
            innerCopy.ProgressChanged += (sender, e) => OnProgressChanged(new ProgressChangedEventArgs(e.Progress * 0.99f));
            innerCopy.Start += (sender, e) => OnStart(e);
            try
            {
                try
                {
                    await innerCopy.Run();
                }
                catch (Exception e) when (!(e is OperationCanceledException)) // Wrap inner exceptions to signal which phase failed
                {
                    throw new CopyFailedException("Exception encountered while copying directory", e);
                }

                cts.Token.ThrowIfCancellationRequested();
                try
                {
                    Directory.Delete(pathFrom, true);
                    OnProgressChanged(new ProgressChangedEventArgs(1.0f));
                }
                catch (Exception e)
                {
                    throw new DeleteFailedException("Exception encountered while removing duplicate files in the old location", e);
                }
            }
            finally
            {
                OnEnd(new EventArgs());
            }
        }
        public class CopyFailedException : Exception
        {
            public CopyFailedException(string message, Exception innerException) : base(message, innerException) { }
        }
        public class DeleteFailedException : Exception
        {
            public DeleteFailedException(string message, Exception innerException) : base(message, innerException) { }
        }
    }
}
