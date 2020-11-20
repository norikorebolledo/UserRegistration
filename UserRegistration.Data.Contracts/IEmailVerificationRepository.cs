using Core.Data.MongoDb;
using Core.Data.MongoDb.Repository;
using System;
using System.Collections.Generic;
using System.Text;
using UserRegistration.Entities;

namespace UserRegistration.Data.Contracts
{
    public interface IEmailVerificationRepository : IMongoRepository<EmailVerification> 
    {
    }
}
