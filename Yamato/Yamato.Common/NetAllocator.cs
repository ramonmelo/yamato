using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Yamato.Common
{
    internal class NetAllocator : IDisposable
    {
        private List<Memory<byte>> memories = new List<Memory<byte>>();

        //public Memory<byte> Allocate(int size)
        //{
        //    var buffer = GC.AllocateArray<byte>(length: size, pinned: true).AsMemory();
        //    memories.Add(buffer);

        //    return buffer;
        //}

        public void Dispose()
        {
        }
    }
}
