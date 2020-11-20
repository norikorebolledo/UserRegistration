using Core.Common.Mail;
using Core.Data.MongoDb;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using UserRegistration.Data;
using UserRegistration.Data.Contracts;
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
            services.Configure<MongoDbSettings>(configuration.GetSection("MongoDbSettings"));
            services.Configure<SecuritySettings>(configuration.GetSection("SecuritySettings"));

            services.AddSingleton<IMongoDbSettings>(serviceProvider => serviceProvider.GetRequiredService<IOptions<MongoDbSettings>>().Value);

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IEmailVerificationRepository, EmailVerificationRepository>();

            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IMailService, MailService>();
            services.AddScoped<IHelperService, HelperService>();

            return services;
        }
    }
}
