using System;
using System.Net;
using System.Threading.Tasks;

namespace Yamato.Common
{
    public interface INetSocket : IDisposable
    {
        Task<bool> Send(EndPoint endPoint, ReadOnlyMemory<byte> buffer);
        Task<Memory<byte>> Receive(EndPoint endPoint, Memory<byte> buffer);
        bool Poll();
        void Bind(EndPoint endPoint);
    }
}
