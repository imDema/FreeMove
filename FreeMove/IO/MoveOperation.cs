using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeMove.IO
{
    class MoveOperation : IOOperation<bool>
    {
        public override Task<bool> Run()
        {
            throw new NotImplementedException();
        }

        public MoveOperation(string pathFrom, string pathTo)
        {

        }
    }
}
