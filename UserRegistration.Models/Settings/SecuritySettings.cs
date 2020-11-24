using System;
using System.Collections.Generic;
using System.Text;

namespace UserRegistration.Models.Settings
{
    public class SecuritySettings
    {
        public string SecretKey { get; set; }
        public int SessionValidityInSeconds { get; set; }
        public int SaltValidityInSeconds { get; set; }
        public int VerificationLimitPerDay { get; set; }
        public bool DevMode { get; set; }
    }
}
