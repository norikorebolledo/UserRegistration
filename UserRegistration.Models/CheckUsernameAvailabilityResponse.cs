using System;
using System.Collections.Generic;
using System.Text;

namespace UserRegistration.Models
{
    public class CheckUsernameAvailabilityResponse : BaseResponse
    {
        public string Username { get; set; }
        public bool Available { get; set; }
    }
}
