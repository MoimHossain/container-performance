

using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace TcpNetFramework
{
    // Sample service
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class MyService : IWcf
    {
        private static int _invocationCounter = 0;
        public string Greet(string name)
        {
            Console.CursorTop = 3;
            Console.CursorLeft = 0;
            Console.WriteLine("Iteration count: {0,22:D8}", ++_invocationCounter);
            return DateTime.Now.ToString() + name;
        }
    }
}
