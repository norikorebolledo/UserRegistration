using Core.Common.Contracts.Date;
using Core.Common.Contracts.Mail;
using Core.Common.Date;
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
            services.AddSingleton<IMailService, MailService>();
            services.AddSingleton<IHelperService, HelperService>();
            services.AddSingleton<IDateTime, AppDateTime>();

            services.AddDbContext<DatabaseContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
            services.Configure<SecuritySettings>(configuration.GetSection("SecuritySettings"));


            services.AddScoped<DbContext, DatabaseContext>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IEmailVerificationRepository, EmailVerificationRepository>();

            services.AddScoped<ILoginService, LoginService>();
            services.AddScoped<IAuthService, AuthService>();
           

            return services;
        }
    }
}
