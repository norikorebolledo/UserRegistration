using Core.Data.MongoDb;
using Core.Data.MongoDb.Repository;
using System;
using System.Collections.Generic;
using System.Text;
using UserRegistration.Data.Contracts;
using UserRegistration.Entities;

namespace UserRegistration.Data
{
    public class EmailVerificationRepository : MongoRepository<EmailVerification>, IEmailVerificationRepository
    {
        public EmailVerificationRepository(IMongoDbSettings settings)
            :base(settings)
        {

        }
    }
}
