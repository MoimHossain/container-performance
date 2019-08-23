using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TcpNetFramework;

namespace MmfIpc
{
    public class ClientApp
    {
        public void Run()
        {
            var file = File.OpenRead(Constants.Persistent.FileName);
            using (MemoryMappedFile mappedFile = MemoryMappedFile.CreateFromFile
                   (file, Constants.Persistent.MapName, file.Length, MemoryMappedFileAccess.Read, null, 0, false))
            {
                using (var accessor = mappedFile.CreateViewAccessor(0, 4))
                {
                    var value = accessor.ReadBoolean(0);

                    Print.Green($"Current value: {value}");
                }
            }

            //using (var mmf = MemoryMappedFile.OpenExisting(Constants.Persistent.MapName))
            //{
            //    using (var accessor = mmf.CreateViewAccessor(0, 4))
            //    {
            //        var value = accessor.ReadBoolean(0);

            //        Print.Green($"Current value: {value}");
            //    }
            //}

            Console.ReadKey();
        }

        public void RunNonPersistentMmf()
        {
            try
            {
                using (var mmf = MemoryMappedFile.OpenExisting(Constants.NonPersistantFileName))
                {
                    var mutex = Mutex.OpenExisting(Constants.MutexName);
                    mutex.WaitOne();

                    using (var stream = mmf.CreateViewStream(1, 0))
                    {
                        var writer = new BinaryWriter(stream);
                        writer.Write(1);
                    }
                    mutex.ReleaseMutex();
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Memory-mapped file does not exist.");
            }
        }
    }
}
