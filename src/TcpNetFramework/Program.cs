using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TcpNetFramework
{
    public class Program
    {
        private static int port = 8789;
        static void Main(string[] args)
        {
            Console.WriteLine("[S] for server [C] for Client");

            switch (Console.ReadKey().Key)
            {
                case ConsoleKey.S: 
                    Server(); break;
                case ConsoleKey.C:
                    Client();
                    break;
            }
        }

        private static void Client()
        {
            try
            {
                // use WcfService.Tcp for NetTcp binding or WcfService.Http for WSHttpBinding

                Console.WriteLine("Enter server IP:");
                var IP = Console.ReadLine();

                if(string.IsNullOrWhiteSpace(IP))
                {
                    IP = Environment.MachineName;
                }

                using (var wcf =
                    WcfService.DefaultFactory.CreateChannel<IWcf>(IP, port, (t) => { return "MyService"; }, "WcfServices"))
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

                        perfCount = count;
                        instance = wcf;

                        //BenchmarkDotNet.Running.BenchmarkRunner.Run<Program>();
                        NetworkThroughputBenchmark.Perform(new Action(new Program().Execute), perfCount);

                        Console.ResetColor();
                        Console.WriteLine("Press [A] to run again...");
                        runAgain = (Console.ReadKey(intercept: true).Key == ConsoleKey.A);

                    } while (runAgain);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static WcfService.ClientProxy<IWcf> instance;
        private static int perfCount;

        [BenchmarkDotNet.Attributes.Benchmark]
        public void Execute()
        {
            try
            {
                var result = instance.Client.Greet("Hello world");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Something went wrong: " + ex.Message);
            }
        }

        private static void Server()
        {
            try
            {
                Console.Clear();
                // use WcfService.Tcp for NetTcp binding or WcfService.Http for WSHttpBinding

                var hosts = WcfService.DefaultFactory.CreateServers(
                    new List<Type> { typeof(MyService) },
                    (t) => { return t.Name; },
                    (t) => { return typeof(IWcf); },
                    "WcfServices",
                    port,
                    (sender, exception) => { Trace.Write(exception); },
                    (msg) => { Trace.Write(msg); },
                    (msg) => { Trace.Write(msg); },
                    (msg) => { Trace.Write(msg); });

                Console.WriteLine($"Server started .... {NetworkUtils.GetLocalIPAddress()}:{port}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            Console.ReadLine();
        }
    }
}
