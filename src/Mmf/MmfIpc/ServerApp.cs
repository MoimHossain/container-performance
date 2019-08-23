using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TcpNetFramework;

namespace MmfIpc
{
    public class ServerApp
    {
        public void Run()
        {
            long offset = 0;
            long length = 4;

            // Create the memory-mapped file.
            using (var mmf = MemoryMappedFile
                .CreateFromFile(Constants.Persistent.FileName, FileMode.OpenOrCreate, Constants.Persistent.MapName, 10000))
            {
                using (var accessor = mmf.CreateViewAccessor(offset, length))
                {
                    accessor.Write(0, true);
                }

                Print.Yellow("Written a Boolean value.");
                Console.ReadLine();

                using (var accessor = mmf.CreateViewAccessor(offset, length))
                {
                    var value = accessor.ReadBoolean(0);
                    var value1 = accessor.ReadBoolean(0);

                    Print.Green($"Current value: {value} and {value1}");
                }
            }

            Console.ReadKey();
        }


        public void RunNonPersistentMmf()
        {
            using (var mmf = MemoryMappedFile.CreateNew(Constants.NonPersistantFileName, 10000))
            {
                bool mutexCreated;
                var mutex = new Mutex(true, Constants.MutexName, out mutexCreated);
                using (var stream = mmf.CreateViewStream())
                {
                    var writer = new BinaryWriter(stream);
                    writer.Write(1);
                }
                mutex.ReleaseMutex();

                Console.WriteLine("Written...waiting to read.");
                Console.ReadLine();

                mutex.WaitOne();
                using (var stream = mmf.CreateViewStream())
                {
                    BinaryReader reader = new BinaryReader(stream);
                    Console.WriteLine("Read 1 says: {0}", reader.ReadBoolean());
                    Console.WriteLine("Read 2 says: {0}", reader.ReadBoolean());
                }
                mutex.ReleaseMutex();
            }
        }
    }
}
