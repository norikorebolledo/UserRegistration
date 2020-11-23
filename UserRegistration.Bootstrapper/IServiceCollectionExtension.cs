using Core.Common.Mail;
using Core.Data.MongoDb;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using UserRegistration.Data;
using UserRegistration.Data.Contracts;
using UserRegistration.Data.Sql;
using UserRegistration.Data.Sql.Repositories;
using UserRegistration.Models.Settings;
using UserRegistration.Service;
using UserRegistration.Service.Contracts;

namespace UserRegistration.Bootstrapper
{
    public static class IServiceCollectionExtension
    {
        public static IServiceCollection RegisterService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddDbContext<DatabaseContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
            services.Configure<SecuritySettings>(configuration.GetSection("SecuritySettings"));


            services.AddScoped<DbContext, DatabaseContext>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IEmailVerificationRepository, EmailVerificationRepository>();

            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IMailService, MailService>();
            services.AddScoped<IHelperService, HelperService>();

            return services;
        }
    }
}
