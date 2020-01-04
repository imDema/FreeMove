using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace FreeMove.IO
{
    public abstract class IOOperation
    {
        public event EventHandler<ProgressChangedEventArgs> ProgressChanged;
        public event EventHandler Start;
        public event EventHandler End;

        protected virtual void OnProgressChanged(ProgressChangedEventArgs e) => ProgressChanged?.Invoke(this, e);
        protected virtual void OnStart(EventArgs e) => Start?.Invoke(this, e);
        protected virtual void OnEnd(EventArgs e) => End?.Invoke(this, e);

        /// <summary>
        /// Stop task as soon as it's safe to do so
        /// </summary>
        public abstract void Cancel();
        /// <summary>
        /// Start the operation
        /// </summary>
        /// <returns>Async task where the operation was started</returns>
        public abstract Task Run();

        public class ProgressChangedEventArgs : EventArgs
        {
            public long Progress { get => progress; }
            public long Max { get => max; }
            long progress;
            long max;
            public ProgressChangedEventArgs(long progress, long max)
            {
                this.progress = progress;
                this.max = max;
            }
        }
    }
}
