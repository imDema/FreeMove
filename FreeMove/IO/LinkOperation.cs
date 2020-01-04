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
