using System;
using System.Collections.Generic;
using System.Text;

namespace UserRegistration.Models
{
    public class LoginSaltResponse : BaseResponse
    {
        public string Username { get; set; }
        public int Validity { get; set; }
        public string Salt { get; set; }
    }
}
