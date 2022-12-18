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
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FreeMove.IO
{
    class MoveOperation : IOOperation
    {
        string pathFrom;
        string pathTo;
        bool sameDrive;
        CancellationTokenSource cts = new CancellationTokenSource();
        CopyOperation innerCopy;
        Form1 form = new Form1();

        public MoveOperation(string pathFrom, string pathTo)
        {
            this.pathFrom = pathFrom;
            this.pathTo = pathTo;
            sameDrive = string.Equals(Path.GetPathRoot(pathFrom), Path.GetPathRoot(pathTo), StringComparison.OrdinalIgnoreCase);
            if (form.chkBox_createDest.Checked && !Directory.Exists(pathTo))
            {
                try
                {
                    Directory.CreateDirectory(Directory.GetParent(pathTo).FullName);
                }
                catch (Exception e) when (e is IOException || e is UnauthorizedAccessException)
                {
                    if (e is UnauthorizedAccessException)
                    {
                        throw new UnauthorizedAccessException("Lacking required permissions to create the destination directory. Try running as administrator.");
                    }
                    else
                    {
                        throw new IOException("Unable to create the destination directory.");
                    }
                }
            }
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
