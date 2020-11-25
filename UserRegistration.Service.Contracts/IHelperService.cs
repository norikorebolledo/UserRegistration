
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UserRegistration.Models;

namespace UserRegistration.Service.Contracts
{
    public interface IHelperService
    {
        string RandomString(int length);
        string ComputeHash(string key, string message);

    }
}
