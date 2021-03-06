﻿

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

        private static double[] PerformAction(Action task, int count)
        {
            var measures = new double[count];
            for (var x = 0; x < count; ++x)
            {
                var stopwatch = Stopwatch.StartNew();
                task();
                stopwatch.Stop();
                measures[x] = stopwatch.Elapsed.TotalMilliseconds;
            }
            return measures;
        }

        public static void ReportMeasures(double[] measures, string caption)
        {
            Print.White("   --------------------------------------------------------------------");
            Print.Cyan($"       {caption}");
            

            var pct = default(double);

            Print.White("   +------------------------------------------------------------------+");
            Print.White("   +   Percentile(%)   |                         Latency              +");
            Print.White("   +------------------------------------------------------------------+");
            for (var i= 10; i <= 90; i += 10)
            {
                PrintCaption($"       * {100-i}th           ");
                pct = Percentile(measures, (double)i/100);
                PrintMeasureUnit(pct, pct);
                Print.White(string.Empty);
            }

            Print.White("   +------------------------------------------------------------------+");
            Print.White("   +   Other           |                         Latency              +");
            Print.White("   +------------------------------------------------------------------+");
            PrintCaption($"       * Outlier");
            pct = Percentile(measures, 1);
            PrintMeasureUnit(pct, pct);
            Print.White(string.Empty);

            PrintCaption("       * Mean:");
            PrintMeasureUnit(measures.Average(), measures.Average());
            Print.White(string.Empty);

            PrintCaption("       * Fastest:");
            PrintMeasureUnit(measures.Min(), 0);
            Print.White(string.Empty);

            Console.ForegroundColor = ConsoleColor.White;
            PrintCaption("       * Slowest:");
            PrintMeasureUnit(measures.Max(), 0);
            Print.White(string.Empty);
        }

        private static void PrintMeasureUnit(double ms, double ns)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("           {0} ns ", (ms * 1000000).ToString().PadLeft(6));
            //if(ns > 0)
            //{
            //    Console.Write("({0} ms)", ns);
            //}
        }

        private static void PrintCaption(string caption)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(caption.PadRight(30));
        }

        public static double Percentile(double[] sequence, double excelPercentile)
        {
            Array.Sort(sequence);
            int N = sequence.Length;
            double n = (N - 1) * excelPercentile + 1;
            // Another method: double n = (N + 1) * excelPercentile;
            if (n == 1d) return sequence[0];
            else if (n == N) return sequence[N - 1];
            else
            {
                int k = (int)n;
                double d = n - k;
                return sequence[k - 1] + d * (sequence[k] - sequence[k - 1]);
            }
        }
    }
}
