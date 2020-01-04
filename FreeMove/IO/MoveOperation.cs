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
        string pathTo;
        bool sameDrive; //TODO: use System.IO.Move if the files are on the same drive
        CancellationTokenSource cts = new CancellationTokenSource();
        CopyOperation innerCopy;

        public MoveOperation(string pathFrom, string pathTo)
        {
            this.pathFrom = pathFrom;
            this.pathTo = pathTo;
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
            innerCopy.ProgressChanged += (sender, e) => OnProgressChanged(e);
            innerCopy.Start += (sender, e) => OnStart(e);
            try
            {
                if (sameDrive)
                {
                    try
                    {
                        await Task.Run(() => Directory.Move(pathFrom, pathTo), cts.Token);
                    }
                    catch (Exception e) when (!(e is OperationCanceledException))
                    {
                        throw new MoveFailedException("Exception encountered while moving on the same drive", e);
                    }
                }
                else
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
                    }
                    catch (Exception e)
                    {
                        throw new DeleteFailedException("Exception encountered while removing duplicate files in the old location", e);
                    }
                }
            }
            finally
            {
                OnEnd(new EventArgs());
            }
        }
        public class MoveFailedException : Exception
        {
            public MoveFailedException(string message, Exception innerException) : base(message, innerException) { }
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
