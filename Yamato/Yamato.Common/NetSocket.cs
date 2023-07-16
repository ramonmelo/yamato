using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Yamato.Common
{
    public class NetSocket : IDisposable
    {
        //private const int RECEIVE_BUFFER_SIZE = 65527;

        private readonly Socket socket;
        //private readonly Memory<byte> recvBuffer;
        private readonly EndPoint recvEndPoint = new IPEndPoint(IPAddress.Any, 0);
        private readonly SocketFlags socketFlags = SocketFlags.None;

        public NetSocket()
        {
            this.socket = new Socket(SocketType.Dgram, ProtocolType.Udp);
            //this.recvBuffer = new byte[RECEIVE_BUFFER_SIZE];
        }

        public void Bind(EndPoint endPoint) => this.socket.Bind(endPoint);
        public bool Poll() => this.socket.Poll(0, SelectMode.SelectRead);

        public void Dispose()
        {
            this.socket.Close();
        }

        public async ValueTask<bool> Send(EndPoint endPoint, ArraySegment<byte> buffer)
        {
            var result = await this.socket.SendToAsync(buffer, socketFlags, endPoint);

            return result > 0;
        }

        public async ValueTask<(EndPoint, int)> Receive(ArraySegment<byte> buffer)
        {
            var result = await this.socket.ReceiveFromAsync(buffer, socketFlags, recvEndPoint);

            //Logger.Debug($"Receive: {nameof(result.ReceivedBytes)}={result.ReceivedBytes}, {nameof(result.ReceivedBytes)}={result.RemoteEndPoint}");

            return (result.RemoteEndPoint, result.ReceivedBytes);
        }
    }
}
