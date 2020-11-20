using System;
using System.Collections.Generic;
using System.Text;

namespace UserRegistration.Models
{
    public class LoginResponse
    {
        public string Command { get; set; }
        public string Username { get; set; }
        public bool Success { get; set; }
        public string SessionID { get; set; }
        public string UserID { get; set; }
        public int Validity { get; set; }
        public string Message { get; set; }
    }
}
