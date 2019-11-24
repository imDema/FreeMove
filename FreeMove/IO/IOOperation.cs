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
        public delegate void ProgressChangedListener(float progress);
        public event ProgressChangedListener ProgressChanged;

        public delegate void OnStartListener();
        public event OnStartListener OnStart;

        public delegate void OnFinishListener();
        public event OnFinishListener OnFinish;

        public delegate void OnCancelListener();
        public abstract event OnCancelListener OnCancel;

        /// <summary>
        /// Stop task as soon as it's safe to do so
        /// </summary>
        public abstract void Cancel();
        /// <summary>
        /// Start the operation
        /// </summary>
        /// <returns>Async task where the operation was started</returns>
        public abstract Task Run();
    }
}
