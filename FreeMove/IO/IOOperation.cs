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
