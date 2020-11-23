using Core.Data.Sql;
using System;

namespace UserRegistration.Entities
{

    public class EmailVerification : Entity<string>
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Code { get; set; }
        public DateTime? CodeSentDate { get; set; }
        public int Counter { get; set; }
        public bool IsVerified { get; set; }
    }
}
