

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TcpNetFramework;

namespace Namedpipe
{
    public class ServerApp
    {

        private  int numThreads = 4;
        private string pipeName;
        public  void Run()
        {
            int i;
            Thread[] servers = new Thread[numThreads];

            Console.WriteLine("\n*** Named pipe server stream example ***\n");

            Print.White("Pipe name: ");
            pipeName = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(pipeName))
            {
                pipeName = "testpipe";
            }

            Print.White("Server: ");Print.Green(NetworkUtils.GetLocalIPAddress());
            Console.WriteLine("Waiting for client connect...\n");
            for (i = 0; i < numThreads; i++)
            {
                servers[i] = new Thread(ServerThread);
                servers[i].Start();
            }
            Thread.Sleep(250);
            while (i > 0)
            {
                for (int j = 0; j < numThreads; j++)
                {
                    if (servers[j] != null)
                    {
                        if (servers[j].Join(250))
                        {
                            Console.WriteLine("Server thread[{0}] finished.", servers[j].ManagedThreadId);
                            servers[j] = null;
                            i--;    // decrement the thread watch count
                        }
                    }
                }
            }
            Console.WriteLine("\nServer threads exhausted, exiting.");
        }

        private  void ServerThread(object data)
        {
            NamedPipeServerStream pipeServer =
                new NamedPipeServerStream(this.pipeName, PipeDirection.InOut, numThreads);

            int threadId = Thread.CurrentThread.ManagedThreadId;

            // Wait for a client to connect
            pipeServer.WaitForConnection();

            Console.WriteLine("Client connected on thread[{0}].", threadId);
            try
            {
                // Read the request from the client. Once the client has
                // written to the pipe its security token will be available.

                StreamString ss = new StreamString(pipeServer);

                // Verify our identity to the connected client using a
                // string that the client anticipates.

                ss.WriteString("I am the one true server!");
                string message = ss.ReadString();

                ss.WriteString($"Server message {DateTime.Now.ToLongTimeString()}");
            }
            // Catch the IOException that is raised if the pipe is broken
            // or disconnected.
            catch (IOException e)
            {
                Console.WriteLine("ERROR: {0}", e.Message);
            }
            pipeServer.Close();
        }
    }
}
