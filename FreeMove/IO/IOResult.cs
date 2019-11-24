using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeMove
{
    class IOResult<E>
    {
        E error;

        private bool ok;
        public bool IsOk { get; }

        public E Error
        {
            get
            {
                if (!ok)
                    return error;
                else
                    throw new InvalidOperationException("Cannot access error of a Result if it's Ok\nCheck for Ok==false before accessing error.");
            }
        }

        public static IOResult<E> Ok()
        {
            return new IOResult<E>()
            {
                ok = true,
            };
        }

        public static  IOResult<E> Err(E error)
        {
            return new IOResult<E>()
            {
                error = error,
                ok = false,
            };
        }

        private IOResult(){}
    }
}
