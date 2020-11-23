using Core.Data.Sql.Repository;
using Microsoft.EntityFrameworkCore;
using UserRegistration.Data.Contracts;
using UserRegistration.Entities;

namespace UserRegistration.Data.Sql.Repositories
{
    public class EmailVerificationRepository : SqlRepository<EmailVerification>, IEmailVerificationRepository
    {
        public EmailVerificationRepository(DbContext context)
            : base(context)
        {

        }
    }
}
