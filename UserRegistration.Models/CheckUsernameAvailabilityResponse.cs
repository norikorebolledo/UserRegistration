using System;
using System.Collections.Generic;
using System.Text;

namespace UserRegistration.Models
{
    public class CheckUsernameAvailabilityResponse
    {
        public string Command { get; set; }
        public string Username { get; set; }
        public bool Available { get; set; }
    }
}
