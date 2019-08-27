

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace UnixDomainSocket.Sample
{
    public class ServerApp
    {
        public void Run(string[] args)
        {
            var host = CreateWebHostBuilder(args).Build();
            host.Start();
            Chmod.Set(Constants.SOCKET_NAME);
            host.WaitForShutdown();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
             WebHost.CreateDefaultBuilder(args)
                 // the following is important, otherwise domain sockets will be unavailable
                 .UseLibuv()
                 // tell Kestrel to create and listen on a domain socket in /tmp
                 .UseKestrel(options =>
                 {
                     options.ListenUnixSocket(Constants.SOCKET_NAME, socketoptions =>
                        {
                            socketoptions.NoDelay = true;
                        });
                 })
                 .UseStartup<Startup>();
    }
}
