using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace GrcRdsServer
{
    class IPHost
    {
        private readonly TcpListener _tcpListener;
        private readonly Thread _listenThread;

        public IPHost()
        {
            _tcpListener = new TcpListener(IPAddress.Any, 22);
            _listenThread = new Thread(ListenForClients);
            _listenThread.Start();
        }

        private void ListenForClients()
        {
            _tcpListener.Start();

            while (true)
            {
                TcpClient client = _tcpListener.AcceptTcpClient();

                var clientThread = new Thread(HandleClientComm);
                clientThread.Start(client);
            }
        }

        private void HandleClientComm(object client)
        {
            var rds = new RdsDemod();

            var tcpClient = (TcpClient)client;
            var clientStream = tcpClient.GetStream();
            var message = new byte[4096];

            while (true)
            {
                try
                {
                    // block until a message
                    int bytesRead = clientStream.Read(message, 0, message.Count());
                    if (bytesRead == 0)
                    {
                        // the client has disconnected
                        break;
                    }

                    var ba = new byte[bytesRead];
                    Array.Copy(message, ba, bytesRead);
                    rds._b.AddRange(ba);
                }
                catch
                {
                    break;
                }

                rds.Process17500RateStreamSamples();
            }

            tcpClient.Close();
        }
    }
}
