using System;
using System.Collections.Generic;
using System.Text;

namespace UserRegistration.Models
{
    public abstract class BaseResponse
    {
        public string Message { get; set; }
        public bool Success { get; set; }
        public string Command { get; set; }
    }
}
