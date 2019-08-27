

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System;

namespace UnixDomainSocket.Sample
{
    class Program
    {
        public static void Main(string[] args)
        {
            Print.Green("Unix Domain Socket");
            Print.Yellow("==========================================================");
            Print.White("Press [S] to run as Server or [C] to run as client.");

            switch (Console.ReadKey(intercept: true).Key)
            {
                case ConsoleKey.S: new ServerApp().Run(args); break;
                case ConsoleKey.C: new ClientApp().Run(); break;
            }
            Console.WriteLine("Application Terminated.");
        }
    }
    public class Constants
    {
        public static string SOCKET_NAME = "/tmp/kestrel.sock";
    }
}
