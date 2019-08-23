using System;
using System.Collections.Generic;
using System.Text;

namespace GrpcNetFramework
{
    public class EmployeeData
    {
        public EmployeeName GetEmployeeName(EmployeeNameRequest request)
        {
            EmployeeName empName = new EmployeeName();
            switch (request.EmpId)
            {
                case "1":
                    empName.FirstName = "Scott";
                    empName.LastName = "Gu";
                    break;
                case "2":
                    empName.FirstName = "Scott";
                    empName.LastName = "Hanselman";
                    break;
                default:
                    empName.FirstName = "Damien";
                    empName.LastName = "Edward";
                    break;
            }
            return empName;
        }
    }
}
