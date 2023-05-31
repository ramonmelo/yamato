using System.Net;
using System.Net.Sockets;

namespace Yamato.Common
{
    public class NetSocket : INetSocket
    {
        private const int RECEIVE_BUFFER_SIZE = 65527;

        private readonly Socket socket;
        private readonly Memory<byte> recvBuffer;
        private readonly EndPoint recvEndPoint = new IPEndPoint(IPAddress.Any, 0);
        private readonly SocketFlags socketFlags = SocketFlags.None;

        public NetSocket()
        {
            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.IPv4);
            this.recvBuffer = new byte[RECEIVE_BUFFER_SIZE];
        }

        public void Bind(EndPoint endPoint) => this.socket.Bind(endPoint);
        public bool Poll() => this.socket.Poll(0, SelectMode.SelectRead);

        public void Dispose()
        {
            this.socket.Close();
            this.socket?.Dispose();
        }

        public async Task<bool> Send(EndPoint endPoint, ReadOnlyMemory<byte> buffer)
        {
            var result = await this.socket.SendToAsync(buffer, socketFlags, endPoint);
            return result > 0;
        }

        public async Task<bool> Receive(EndPoint endPoint, Memory<byte> buffer)
        {
            var result = await this.socket.ReceiveFromAsync(recvBuffer, socketFlags, recvEndPoint);

            endPoint = result.RemoteEndPoint;
            buffer = recvBuffer.Slice(0, result.ReceivedBytes);

            return result.ReceivedBytes > 0;
        }
    }
}
