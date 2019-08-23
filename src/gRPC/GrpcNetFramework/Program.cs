using Grpc.Core;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcpNetFramework;

namespace GrpcNetFramework
{
    public class Program
    {
        private static int DEFAULTPORT = 7888;
        private static int PORT = 7888;

        private static bool RunningWithArguments(string[] args)
        {
            return args != null && args.Length > 0;
        }

        static void Main(string[] args)
        {
            bool withArgs = RunningWithArguments(args);

            Console.WriteLine("Net Framework - gRPC Demo");
            Console.WriteLine("==========================================================");
            Console.WriteLine("Press [S] to run as Server or [C] to run as client.");

            if (withArgs)
            {
                if (args[0].Equals("server", StringComparison.OrdinalIgnoreCase))
                {
                    RunAsServer(args);
                }
                else
                {
                    RunAsClient(args);
                }
            }
            else
            {
                switch (Console.ReadKey(intercept: true).Key)
                {
                    case ConsoleKey.S: RunAsServer(args); break;
                    case ConsoleKey.C: RunAsClient(args); break;
                }
            }
            Console.WriteLine("Application Terminated.");
        }

        private static void RunAsClient(string[] args)
        {
            bool withArgs = RunningWithArguments(args);
            try
            {
                var hostName = ReadHostName(args);
                ReadPort(args);
                ShowAppHeader(args, false, hostName);

                var channel = new Channel($"{hostName}:{PORT}", ChannelCredentials.Insecure);
                var client = new AccountService.AccountServiceClient(channel);

                var count = 100;
                var runAgain = false;
                do
                {
                    ShowAppHeader(args, false);

                    if (withArgs)
                    {
                        if (args.Length >= 4)
                        {
                            if (!Int32.TryParse(args[4], out count))
                            {
                                count = 10;
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("How many iterations you want to perform?");
                        if (!Int32.TryParse(Console.ReadLine(), out count))
                        {
                            count = 10;
                        }
                    }

                    accClient = client;
                    perfCount = count;

                    NetworkThroughputBenchmark.Perform(new Action(new Program().RunBenchmark), perfCount);

                    if (!withArgs)
                    {
                        Console.WriteLine("Press [A] to run again...");
                        runAgain = (Console.ReadKey(intercept: true).Key == ConsoleKey.A);
                    }
                } while (runAgain);

                channel.ShutdownAsync().Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception encountered: {ex}");
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey(intercept: true);
            }
        }

        private static AccountService.AccountServiceClient accClient;
        private static int perfCount = 0;


        public void RunBenchmark()
        {
            var empName = accClient.GetEmployeeName(new EmployeeNameRequest { EmpId = "1" });
        }

        private static void RunAsServer(string[] args)
        {
            try
            {
                ReadPort(args);
                ShowAppHeader(args, true);

                var server = new Grpc.Core.Server
                {
                    Services = { AccountService.BindService(new AccountsImpl()) },
                    Ports = { new ServerPort(NetworkUtils.GetLocalIPAddress(), PORT, ServerCredentials.Insecure) }
                };



                server.Start();
                Console.ReadLine();

                Console.WriteLine("Terminating...");
                server.ShutdownAsync().Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception encountered: {ex}");
                Console.ReadKey(intercept: true);
            }
        }

        private static void ShowAppHeader(string[] args, bool server = true, string hostName = "")
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"GRPC {(server ? "Server" : "Client")}");
            Console.WriteLine("===================================================");
            Console.WriteLine($"{(server ? $"Server listening to {NetworkUtils.GetLocalIPAddress()}:{PORT}" : $"Client connecting to {hostName}:{PORT}")}");
            Console.ResetColor();
        }

        private static void ReadPort(string[] args)
        {
            bool withArgs = RunningWithArguments(args);

            if (withArgs)
            {
                if (args.Length >= 2 && !Int32.TryParse(args[1], out PORT))
                {
                    PORT = DEFAULTPORT;
                }
            }
            else
            {
                Console.WriteLine($"Please specify a PORT (defaults to {DEFAULTPORT})");
                if (!Int32.TryParse(Console.ReadLine(), out PORT))
                {
                    PORT = DEFAULTPORT;
                }
            }
        }

        private static string ReadHostName(string[] args)
        {
            bool withArgs = RunningWithArguments(args);

            var hostName = "127.0.0.1";

            if (withArgs)
            {
                if (args.Length >= 3) return args[3];
                return hostName;
            }
            else
            {
                Console.WriteLine("Please enter the Server host name. (defaults to 127.0.0.1)");

                var x = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(x))
                {
                    return hostName;
                }
                return x;
            }
        }
    }
}
