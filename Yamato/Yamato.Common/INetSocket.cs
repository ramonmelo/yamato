using System.Net;

namespace Yamato.Common
{
    public interface INetSocket : IDisposable
    {
        Task<bool> Send(EndPoint endPoint, ReadOnlyMemory<byte> buffer);
        Task<bool> Receive(EndPoint endPoint, Memory<byte> buffer);

        bool Poll();
        void Bind(EndPoint endPoint);
    }
}
