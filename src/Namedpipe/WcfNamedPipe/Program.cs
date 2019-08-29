

using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;
using TcpNetFramework;

namespace WcfNamedPipe
{
    class Program
    {
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
        private static ServiceProxy instance;

        private static void Client()
        {
            try
            {

                Console.WriteLine("Enter server IP:");
                var IP = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(IP))
                {
                    IP = Environment.MachineName;
                }

                var wcf = new ServiceProxy();

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

                        instance = wcf;

                        //BenchmarkDotNet.Running.BenchmarkRunner.Run<Program>();
                        NetworkThroughputBenchmark.Perform(new Action(new Program().Execute), count);

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

        public void Execute()
        {
            try
            {
                var result = instance.InvokeHelloWorld("Hello world");
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
                var serviceHost = new ServiceHost
                        (typeof(MyService), new Uri[] { new Uri("net.pipe://localhost/MyAppNameThatNobodyElseWillUse") });
                serviceHost.AddServiceEndpoint(typeof(IWcf), new NetNamedPipeBinding(), "helloservice");
                serviceHost.Open();

                Console.WriteLine("Service started. Available in following endpoints");
                foreach (var serviceEndpoint in serviceHost.Description.Endpoints)
                {
                    Console.WriteLine(serviceEndpoint.ListenUri.AbsoluteUri);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            Console.ReadLine();
        }
    }

    public class ServiceProxy : ClientBase<IWcf>
    {
        public ServiceProxy()
            : base(new ServiceEndpoint(ContractDescription.GetContract(typeof(IWcf)),
                new NetNamedPipeBinding(), new EndpointAddress("net.pipe://localhost/MyAppNameThatNobodyElseWillUse/helloservice")))
        {

        }
        public string InvokeHelloWorld(string message)
        {
            return Channel.Greet(message);
        }
    }
}
