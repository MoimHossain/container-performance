

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TcpNetFramework
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
            if(ns > 0)
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
}
