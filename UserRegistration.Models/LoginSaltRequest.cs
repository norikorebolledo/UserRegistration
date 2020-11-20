using System;
using System.Collections.Generic;
using System.Text;

namespace UserRegistration.Models
{
    public class LoginSaltRequest
    {
        public string Command { get; set; }
        public string Username { get; set; }
    }
}
