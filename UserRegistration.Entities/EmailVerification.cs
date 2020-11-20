using Core.Data.MongoDb;
using Core.Data.MongoDb.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace UserRegistration.Entities
{
    [BsonCollection("emailVerifications")]
    public class EmailVerification : Document
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Code { get; set; }
        public DateTime? CodeSentDate { get; set; }
        public int Counter { get; set; }
        public bool IsVerified { get; set; }
    }
}
