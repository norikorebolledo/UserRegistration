using Core.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using UserRegistration.Service.Contracts;

namespace UserRegistration.Service
{
    public class HelperService : IHelperService
    {
        public string RandomString(int length)
        {
            return SecurityHelper.RandomString(length);
        }

        public string ComputeHash(string key, string message)
        {
            return SecurityHelper.ComputeHash(key, message);
        }
    }
}
