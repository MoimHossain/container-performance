using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TcpNetFramework;

namespace Namedpipe
{
    public class ClientApp
    {
        public void Run()
        {
            Print.White("Server IP: ");
            var ip = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(ip))
            {
                ip = ".";
            }

            Print.White("Pipe name: ");
            var pipeName = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(pipeName))
            {
                pipeName = "testpipe";
            }
            try
            {
                RunCore(ip, pipeName);
                // Give the client process some time to display results before exiting.
                Thread.Sleep(4000);
            }
            catch (Exception ex)
            {
                Print.Red(ex.Message);
            }
            Console.ResetColor();            
        }

        private static void RunCore(string ip, string pipeName)
        {
            NamedPipeClientStream pipeClient =
                    new NamedPipeClientStream(ip, pipeName,
                        PipeDirection.InOut, PipeOptions.None,
                        TokenImpersonationLevel.Impersonation);

            Console.WriteLine("Connecting to server...\n");
            pipeClient.Connect();

            StreamString ss = new StreamString(pipeClient);
            // Validate the server's signature string
            if (ss.ReadString() == "I am the one true server!")
            {
                // The client security token is sent with the first write.
                // Send the name of the file whose contents are returned
                // by the server.
                ss.WriteString("Message from client " + DateTime.Now.ToString());

                // Print the file to the screen.
                Console.Write(ss.ReadString());
            }
            else
            {
                Console.WriteLine("Server could not be verified.");
            }
            pipeClient.Close();
        }
    }
}
