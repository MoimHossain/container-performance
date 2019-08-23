

using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace TcpNetFramework
{
    [ServiceContract(Namespace = "http://abc.com/enterpriseservices")]
    public interface IWcf
    {
        [OperationContract]
        string Greet(string name);
    }
}
