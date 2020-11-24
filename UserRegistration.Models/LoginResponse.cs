using System;
using System.Collections.Generic;
using System.Text;

namespace UserRegistration.Models
{
    public class LoginResponse: BaseResponse
    {
        public string Username { get; set; }
        public string SessionID { get; set; }
        public string UserID { get; set; }
        public int Validity { get; set; }
    }
}
