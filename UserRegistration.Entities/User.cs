using Core.Data.MongoDb;
using Core.Data.MongoDb.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace UserRegistration.Entities
{
    [BsonCollection("users")]
    public class User : Document
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public string Salt { get; set; }
        public int SaltValidity { get; set; }
        public DateTime? SaltGenerationDate { get; set; }
    }
}
