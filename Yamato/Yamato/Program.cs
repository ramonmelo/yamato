using System.Buffers;
using System.Net;
using System.Net.Sockets;
using Yamato.App;
using Yamato.Common;

internal class Program
{
    /// <summary>
    /// Change this number to change the amount of data we send at once.
    /// </summary>
    private const int PacketSize = 1280;
    private static readonly IPEndPoint _blankEndpoint = new IPEndPoint(IPAddress.Any, 0);

    static async Task Main(string[] args)
    {
        //using var udpSocket = new Socket(SocketType.Dgram, ProtocolType.Udp);

        using var udpSocket = new NetSocket();

        // Get a cancel source that cancels when the user presses CTRL+C.
        var userExitSource = GetUserConsoleCancellationSource();

        var cancelToken = userExitSource.Token;

        // Discard our socket when the user cancels.
        using var cancelReg = cancelToken.Register(() => udpSocket.Dispose());

        var throughput = new ThroughputCounter();

        // Start a background task to print throughput periodically.
        _ = PrintThroughput(throughput, cancelToken);

        // Client or server?
        if (args.Length > 0 && args[0] == "-c")
        {
            // Client.
            if (args.Length > 1 && IPAddress.TryParse(args[1], out var destination))
            {
                Console.WriteLine($"Sending to {destination}:9999");
                await DoSendAsync(udpSocket, new IPEndPoint(destination, 9999), throughput, cancelToken);
            }
            else
            {
                Console.WriteLine("-c argument requires an IP address");
            }
        }
        else
        {
            // Server.
            udpSocket.Bind(new IPEndPoint(IPAddress.Any, 9999));

            Console.WriteLine("Listening on 0.0.0.0:9999");
            Console.WriteLine("Run with -c <ip address> to be a client.");
            await DoReceiveAsync(udpSocket, throughput, cancelToken);
        }
    }

    private static async Task PrintThroughput(ThroughputCounter counter, CancellationToken cancelToken)
    {
        while (!cancelToken.IsCancellationRequested)
        {
            await Task.Delay(1000, cancelToken);

            var count = counter.SampleAndReset();

            var megabytes = count / 1024d / 1024d;

            double pps = count / PacketSize;

            Console.WriteLine("{0:0.00}MBps ({1:0.00}Mbps) - {2:0.00}pps", megabytes, megabytes * 8, pps);
        }
    }


    private static async Task DoSendAsync(NetSocket udpSocket, IPEndPoint destination, ThroughputCounter throughput, CancellationToken cancelToken)
    {
        // Taking advantage of pre-pinned memory here using the .NET 5 POH (pinned object heap).            
        byte[] buffer = GC.AllocateArray<byte>(PacketSize, pinned: true);
        //Memory<byte> bufferMem = buffer.AsMemory();

        var bufferMemory = new ArraySegment<byte>(buffer);

        // Put something approaching meaningful data in the buffer.
        for (var idx = 0; idx < PacketSize; idx++)
        {
            bufferMemory[idx] = (byte)idx;

            //bufferMem.Span[idx] = (byte)idx;
        }

        while (!cancelToken.IsCancellationRequested)
        {
            //await udpSocket.SendToAsync(bufferMem, SocketFlags.None, destination, cancelToken);

            await udpSocket.Send(destination, bufferMemory);

            throughput.Add(bufferMemory.Count);

            //throughput.Add(bufferMem.Length);
        }
    }

    private static async Task DoReceiveAsync(NetSocket udpSocket, ThroughputCounter throughput, CancellationToken cancelToken)
    {
        // Taking advantage of pre-pinned memory here using the .NET5 POH (pinned object heap).
        byte[] buffer = GC.AllocateArray<byte>(length: 65527, pinned: true);

        //Memory<byte> bufferMem = buffer.AsMemory();
        //Memory<byte> memory = new Memory<byte>(buffer);

        var bufferMemory = new ArraySegment<byte>(buffer);

        while (!cancelToken.IsCancellationRequested)
        {
            try
            {
                var result = await udpSocket.Receive(bufferMemory);

                Console.WriteLine($"{result.Item1}: {result.Item2}");

                //var result = await udpSocket.ReceiveFromAsync(bufferMem, SocketFlags.None, _blankEndpoint);

                throughput.Add(result.Item2);
            }
            catch (SocketException)
            {
                // Socket exception means we are finished.
                break;
            }
        }
    }

    private static CancellationTokenSource GetUserConsoleCancellationSource()
    {
        var cancellationSource = new CancellationTokenSource();

        Console.CancelKeyPress += (sender, args) =>
        {
            args.Cancel = true;
            cancellationSource.Cancel();
        };

        return cancellationSource;
    }
}