using System;
using System.Collections.Generic;
using System.Text;

namespace UserRegistration.Models
{
    public class VerificationResponse
    {
        public string Command { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
