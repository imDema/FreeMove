using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FreeMove.IO
{
    class LinkOperation : IOOperation
    {
        readonly string directory;
        readonly string symlink;
        CancellationTokenSource cts = new CancellationTokenSource();
        public override void Cancel()
        {
            cts.Cancel();
        }

        public override Task Run()
        {
            return Task.Run(() =>
            {
                return IOHelper.MakeLink(directory, symlink);
            }, cts.Token);
        }

        public LinkOperation(string directory, string symlink)
        {
            this.directory = directory;
            this.symlink = symlink;
        }
    }
}
