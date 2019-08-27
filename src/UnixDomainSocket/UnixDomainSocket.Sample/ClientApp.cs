using System;


using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace UnixDomainSocket.Sample
{
    public class ClientApp
    {
        public void Run()
        {
            try
            {
                var payload = new byte[1024];
                var unixSocket = Constants.SOCKET_NAME;
                var socket = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.IP);
                var unixEp = new UnixEndPoint(unixSocket);
                socket.NoDelay = true;
                socket.Connect(unixEp);

                socket.SendBufferSize = payload.Length;
                var signal = socket.Send(payload, SocketFlags.None);
                Print.Yellow($"Response from server: {signal}");

                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Print.Red(ex.Message);
            }
        }
    }
}
