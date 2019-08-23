

using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.Owin.Cors;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalR
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(CorsOptions.AllowAll);
            app.MapSignalR();
        }
    }

    [HubName("ChatHub")]
    public class ChatHub : Hub
    {
        private static int _invocationCounter = 0;
        public void Send(string name, string message)
        {
            Console.CursorTop = 3;
            Console.CursorLeft = 0;
            Console.WriteLine("Iteration count: {0,22:D8}", ++_invocationCounter);

            //Clients.All.addMessage(name, message);
        }
    }
}
