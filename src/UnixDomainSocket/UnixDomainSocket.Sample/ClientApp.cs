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
                var count = 100;
                var runAgain = false;
                do
                {
                    Console.Clear();
                    Console.WriteLine("How many iterations you want to perform?");
                    if (!Int32.TryParse(Console.ReadLine(), out count))
                    {
                        count = 10;
                    }

                    NetworkThroughputBenchmark.Perform(new Action(this.SendRequestOverSocket), count);

                    Console.ResetColor();
                    Console.WriteLine("Press [A] to run again...");
                    runAgain = (Console.ReadKey(intercept: true).Key == ConsoleKey.A);

                } while (runAgain);
            }
            catch (Exception ex)
            {
                Print.Red(ex.Message);
            }
        }

        private void SendRequestOverSocket()
        {
            var unixSocket = Constants.SOCKET_NAME;
            using (var socket = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.IP))
            {
                var unixEp = new UnixEndPoint(unixSocket);
                //socket.NoDelay = true;
                socket.Connect(unixEp);

                var header = "POST /index HTTP/1.1\r\nHost: 127.0.0.1\r\nContent-Type: application/x-www-form-urlencoded; charset=UTF-8\r\n";
                var body = "hello=workd&aid=SomeID\r\n";
                var cl = "Content-Length: " + (Encoding.UTF8.GetByteCount(body)) + "\r\n\r\n";
                var payload = Encoding.UTF8.GetBytes(header + cl + body);
                socket.Send(payload, payload.Length, 0);
                socket.Close();
            }
        }
    }
}

// Working GET
//public void Run()
//{
//    try
//    {
//        var payload = new byte[1024];
//        var unixSocket = Constants.SOCKET_NAME;
//        var socket = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.IP);
//        var unixEp = new UnixEndPoint(unixSocket);
//        //socket.NoDelay = true;
//        socket.Connect(unixEp);




//        var request = "GET /index HTTP/1.1\r\n" +
//            "Host: 127.0.0.1\r\n" +
//            "Content-Length: 0\r\n" +
//            "\r\n";
//        //var requestBuilder = new StringBuilder();
//        //requestBuilder.AppendLine("GET /index HTTP/1.1");
//        //requestBuilder.AppendLine("HOST: 127.0.0.1");
//        //requestBuilder.AppendLine("Content-Length: 0");
//        //requestBuilder.AppendLine();

//        payload = Encoding.ASCII.GetBytes(request);

//        socket.SendBufferSize = payload.Length;
//        var signal = socket.Send(payload, SocketFlags.None);

//        Print.Yellow($"Response from server: {signal}");


//        socket.Close();
//        Console.ReadKey();
//    }
//    catch (Exception ex)
//    {
//        Print.Red(ex.Message);
//    }
//}
