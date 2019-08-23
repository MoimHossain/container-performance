

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
    public class NetworkUtils
    {
        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            return Environment.MachineName;
        }
    }

    public class Print
    {
        public static void Green(string message, bool skipNewLine = false)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            PrintCore(message, skipNewLine);
        }

        public static void Red(string message, bool skipNewLine = false)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            PrintCore(message, skipNewLine);
        }

        public static void Yellow(string message, bool skipNewLine = false)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            PrintCore(message, skipNewLine);
        }

        public static void White(string message, bool skipNewLine = false)
        {
            Console.ForegroundColor = ConsoleColor.White;
            PrintCore(message, skipNewLine);
        }

        public static void Cyan(string message, bool skipNewLine = false)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            PrintCore(message, skipNewLine);
        }

        public static void Blue(string message, bool skipNewLine = false)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            PrintCore(message, skipNewLine);
        }

        public static void DarkGray(string message, bool skipNewLine = false)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            PrintCore(message, skipNewLine);
        }

        private static void PrintCore(string message, bool skipNewLine)
        {
            if (skipNewLine)
            {
                Console.Write(message);
            }
            else
            {
                Console.WriteLine(message);
            }
        }
    }
}


