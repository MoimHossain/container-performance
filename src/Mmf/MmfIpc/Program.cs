using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcpNetFramework;

namespace MmfIpc
{
    class Program
    {
        static void Main(string[] args)
        {
            PrintHeader();

            switch (Console.ReadKey().Key)
            {
                case ConsoleKey.S:
                    Console.Clear();
                    new ServerApp().Run();
                    break;
                case ConsoleKey.C:
                    Console.Clear();
                    new ClientApp().Run();
                    break;
            }
            Print.White("Application terminated. Press any key.");
            Console.ReadKey();
        }

        private static void PrintHeader()
        {
            Console.Clear();
            Print.White("Memory Mapped File (MMF)");
            Print.Yellow("=============================================================");
            Print.White(string.Empty);
            Print.White("Press [S] for server, [C] for client..");
        }
    }
}
