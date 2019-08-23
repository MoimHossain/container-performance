

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Grpc.Core;

namespace GrpcNetFramework
{
    public class AccountsImpl : AccountService.AccountServiceBase
    {
        private static int _invocationCounter = 0;

        public override Task<EmployeeName> GetEmployeeName(EmployeeNameRequest request, ServerCallContext context)
        {
            Console.CursorTop = 3;
            Console.CursorLeft = 0;
            Console.WriteLine("Iteration count: {0,22:D8}", ++_invocationCounter);
            return Task.FromResult(new EmployeeData().GetEmployeeName(request));
        }
    }
}
