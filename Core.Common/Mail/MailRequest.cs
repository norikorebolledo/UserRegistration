﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Common.Mail
{
    public class MailRequest
    {
        public string ToEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}
