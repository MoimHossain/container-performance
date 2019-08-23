

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MmfIpc
{
    public class Constants
    {
        public const string NonPersistantFileName = "NPMMF2019";
        public const string MutexName = "NPMMFMUTEX2019";


        public class Persistent
        {
            public const string FileName = @"C:\Temp\PMMF\mmf.data";
            public const string MapName = "DataMap";
        }
    }
}
