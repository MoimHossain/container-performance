using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace UnixDomainSocket.Sample
{
    public class NetworkThroughputBenchmark
    {
        public static void Perform(Action task, int count)
        {
            var warmupMeasures = PerformAction(task, count);
            ReportMeasures(warmupMeasures, "Warmup");

            var actualMeasures = PerformAction(task, count);
            ReportMeasures(actualMeasures, "Actual");
        }

        private static long[] PerformAction(Action task, int count)
        {
            var measures = new long[count];
            for (var x = 0; x < count; ++x)
            {
                var stopwatch = Stopwatch.StartNew();
                task();
                stopwatch.Stop();
                measures[x] = stopwatch.ElapsedMilliseconds;
            }
            return measures;
        }

        private static void ReportMeasures(long[] measures, string caption)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("======================================================================");
            Console.WriteLine(caption);
            Console.WriteLine("======================================================================");
            PrintCaption("Mean:");
            PrintMeasureUnit(System.Convert.ToInt64(measures.Average()), measures.Average());

            PrintCaption("Fastest:");
            PrintMeasureUnit(System.Convert.ToInt64(measures.Min()), 0);

            Console.ForegroundColor = ConsoleColor.White;
            PrintCaption("Slowest:");
            PrintMeasureUnit(System.Convert.ToInt64(measures.Max()), 0);

            PrintCaption("Total:");
            PrintMeasureUnit(System.Convert.ToInt64(measures.Sum()), 0);
        }

        private static void PrintMeasureUnit(long ms, double ns)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("{0,22:D16} ms ", ms, ns);
            if (ns > 0)
            {
                Console.Write("({0} ns)", ns);
            }
            Console.WriteLine();
        }

        private static void PrintCaption(string caption)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(caption.PadRight(30));
        }
    }

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
