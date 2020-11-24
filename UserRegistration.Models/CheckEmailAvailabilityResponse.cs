using System;
using System.Collections.Generic;
using System.Text;

namespace UserRegistration.Models
{
    public class CheckEmailAvailabilityResponse : BaseResponse
    {
        public string Email { get; set; }
        public bool Available { get; set; }
    }
}
