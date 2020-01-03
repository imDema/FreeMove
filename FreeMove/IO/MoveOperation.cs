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
            innerCopy.Completed += (sender, e) => OnCompleted(e);
        }

        public override Task Run()
        {
            innerCopy.ProgressChanged += (sender, e) => OnProgressChanged(new ProgressChangedEventArgs(e.Progress * 0.99f));
            innerCopy.Start += (sender, e) => OnStart(e);

            Task copyTask = innerCopy.Run();

            Task deleteTask = copyTask.ContinueWith(t =>
            {
                try
                {
                    Directory.Delete(pathFrom, true);
                    OnProgressChanged(new ProgressChangedEventArgs(1.0f));
                }
                catch (Exception e)
                {
                    throw new DeleteFailedException("Exception encountered while removing duplicate files in the old location", e);
                }
            }, TaskContinuationOptions.OnlyOnRanToCompletion);


            Task faultedTask = copyTask.ContinueWith(t =>
            {
                if (t.IsFaulted)
                    throw new CopyFailedException("Exception encountered while copying directory", t.Exception);
                else if (t.IsCanceled)
                    throw new OperationCanceledException();

            }, TaskContinuationOptions.NotOnRanToCompletion);

            return Task.WhenAny(new Task[] { deleteTask, faultedTask });
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
