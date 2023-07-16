using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Yamato.Common
{
    internal class NetManager : IDisposable
    {
        public NetConfig NetConfig { get; }
        private NetSocket Socket { get; }
        private List<NetConnection> Connections { get; }
        private CancellationTokenSource CancellationTokenSource { get; }

        public NetManager(NetConfig netConfig)
        {
            this.NetConfig = netConfig;

            this.Socket = new NetSocket();
            this.Connections = new List<NetConnection>();

            this.CancellationTokenSource = new CancellationTokenSource();
        }

        public void StartNetwork()
        {
            this.Socket.Bind(this.NetConfig.BindingAddress);

            // Run network loop
            NetworkLoop(this.CancellationTokenSource.Token);
        }

        private async void NetworkLoop(CancellationToken token)
        {
            try
            {
                while (true)
                {
                    token.ThrowIfCancellationRequested();
                    //this.Socket.Receive()
                }
            }
            catch (OperationCanceledException)
            {
                Logger.Debug("Network Loop stopped");
            }
            catch (Exception e)
            {
                Logger.Error(e.Message);
            }
        }

        public void Dispose()
        {
            this.CancellationTokenSource?.Cancel();
            this.Socket?.Dispose();
        }
    }
}
