using Microsoft.Owin.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcpNetFramework;

namespace SignalR
{
    class Program
    {
        public static int PORT = 8790;

        static void Main(string[] args)
        {
            Print.Green("Net Framework - SignalR Demo");
            Print.Yellow("==========================================================");
            Print.White("Press [S] to run as Server or [C] to run as client.");

            switch (Console.ReadKey(intercept: true).Key)
            {
                case ConsoleKey.S: new ServerApp().Run(); break;
                case ConsoleKey.C: new ClientApp().RunAsync().Wait(); break;
            }
            Console.WriteLine("Application Terminated.");            
        }
    }
}
