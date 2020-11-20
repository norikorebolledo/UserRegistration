using System;
using System.Collections.Generic;
using System.Text;

namespace UserRegistration.Models
{
    public class LoginSaltResponse
    {
        public string Command { get; set; }
        public string Username { get; set; }
        public int Validity { get; set; }
        public string Salt { get; set; }
        public string Message { get; set; }
        public bool Success { get; set; }
    }
}
