using Core.Data.MongoDb;
using Core.Data.MongoDb.Repository;
using UserRegistration.Data.Contracts;
using UserRegistration.Entities;

namespace UserRegistration.Data.MongoDb.Repositories
{
    public class UserRepository : MongoRepository<User>, IUserRepository
    {
        public UserRepository(IMongoDbSettings settings)
            :base(settings)
        {

        }
    }
}
