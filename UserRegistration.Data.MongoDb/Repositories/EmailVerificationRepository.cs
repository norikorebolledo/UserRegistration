using Core.Data.MongoDb;
using Core.Data.MongoDb.Repository;
using UserRegistration.Data.Contracts;
using UserRegistration.Entities;

namespace UserRegistration.Data.MongoDb.Repositories
{
    public class EmailVerificationRepository : MongoRepository<EmailVerification>, IEmailVerificationRepository
    {
        public EmailVerificationRepository(IMongoDbSettings settings)
            :base(settings)
        {

        }
    }
}
