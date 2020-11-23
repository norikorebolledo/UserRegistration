using Core.Data.Sql.Repository;
using Microsoft.EntityFrameworkCore;
using UserRegistration.Data.Contracts;
using UserRegistration.Entities;

namespace UserRegistration.Data.Sql.Repositories
{

    public class UserRepository : SqlRepository<User>, IUserRepository
    {
        public UserRepository(DbContext context)
            : base(context)
        {

        }
    }
}
