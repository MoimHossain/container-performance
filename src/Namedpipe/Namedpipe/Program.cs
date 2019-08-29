using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Namedpipe
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("[S] for server [C] for Client");

            switch (Console.ReadKey().Key)
            {
                case ConsoleKey.S:
                    new ServerApp().Run(); break;
                case ConsoleKey.C:
                    new ClientApp().Run();
                    break;
            }
        }
    }
}
