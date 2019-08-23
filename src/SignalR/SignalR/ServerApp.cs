using Microsoft.Owin.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcpNetFramework;

namespace SignalR
{
    public class ServerApp
    {
        public void Run()
        {
            // This will *ONLY* bind to localhost, if you want to bind to all addresses
            // use http://*:8080 to bind to all addresses. 
            // See http://msdn.microsoft.com/library/system.net.httplistener.aspx 
            // for more information.
            var url = $"http://{NetworkUtils.GetLocalIPAddress()}:{Program.PORT}";
            using (WebApp.Start(url))
            {
                Print.White(Environment.NewLine);
                Print.Green($"Server running on {url}");
                Console.ReadLine();
            }
        }
    }
}
