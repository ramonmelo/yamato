using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yamato.App
{
    internal class ThroughputCounter
    {
        private long _deltaCount;

        public void Add(long value)
        {
            Interlocked.Add(ref _deltaCount, value);
        }

        public long SampleAndReset()
        {
            return Interlocked.Exchange(ref _deltaCount, 0);
        }
    }
}
