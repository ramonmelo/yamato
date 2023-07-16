using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Yamato.Common
{
    internal class NetConfig
    {
        public EndPoint BindingAddress { get; set; }
        public int PacketSize { get; set; }
    }
}
