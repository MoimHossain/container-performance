

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace UnixDomainSocket.Sample
{
    public class Chmod
    {
        [DllImport("libc", SetLastError = true)]
        private static extern int chmod(string pathname, int mode);

        // user permissions
        const int S_IRUSR = 0x100;
        const int S_IWUSR = 0x80;
        const int S_IXUSR = 0x40;

        // group permission
        const int S_IRGRP = 0x20;
        const int S_IWGRP = 0x10;
        const int S_IXGRP = 0x8;

        // other permissions
        const int S_IROTH = 0x4;
        const int S_IWOTH = 0x2;
        const int S_IXOTH = 0x1;

        public static void Set(string filename)
        {
            const int _0755 =
                S_IRUSR | S_IXUSR | S_IWUSR
                | S_IRGRP | S_IXGRP | S_IWGRP
                | S_IROTH | S_IXOTH | S_IWOTH;
            Console.WriteLine(Path.GetFullPath(filename));
            Console.WriteLine("Result = " + chmod(Path.GetFullPath(filename), (int)_0755));
        }
    }
}
