

using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcpNetFramework;

namespace SignalR
{
    public class ClientApp
    {
        public async Task RunAsync()
        {
            Console.Clear();
            Print.White("Please enter server IP:", true);
            Print.Cyan("", true);
            var address = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(address))
            {
                address = NetworkUtils.GetLocalIPAddress();
            }            

            var connection = new HubConnection($"http://{address}:{Program.PORT}/");
            var myHub = connection.CreateHubProxy("ChatHub");

            await connection.Start().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Print.Red($"There was an error opening the connection:{task.Exception.GetBaseException()}");
                }
                else
                {
                    Print.Green("Server connection established...");
                }
            });

            myHub.On<string, string>("addMessage", (who, message) =>
            {
                Print.Blue($"{who} :: {message}");
            });

            var count = 100;
            var runAgain = false;
            do
            {
                Print.White("How many iterations you want to perform?", true); Print.Cyan("");
                if (!Int32.TryParse(Console.ReadLine(), out count))
                {
                    count = 10;
                }
                NetworkThroughputBenchmark.Perform(new Action(() =>
                {
                    myHub.Invoke<string>("Send", address, DateTime.Now.Ticks).Wait();
                }), count);

                Print.White("Press [A] to run again...");
                runAgain = (Console.ReadKey(intercept: true).Key == ConsoleKey.A);

            } while (runAgain);

            connection.Stop();
        }        
    }
}
