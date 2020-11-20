using System;
using System.Collections.Generic;
using System.Text;

namespace UserRegistration.Models
{
    public class CheckEmailAvailabilityRequest
    {
        public string Command { get; set; }
        public string Email { get; set; }
    }
}
