using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcpNetFramework;

namespace MeasureFormulaTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var items = new long[] {
                10000, 10002, 10000, 10009, 10001, 10001, 10000, 10000, 10002, 10001,
                10000, 10002, 10000, 10002, 10001, 10001, 10000, 10000, 10002, 10001};

            //var value = new NetworkThroughputBenchmark().Percentile(items, 0.9);

            //Console.WriteLine(value);

            NetworkThroughputBenchmark.ReportMeasures(items, "test");

            Console.ReadKey();
        }
    }
}
