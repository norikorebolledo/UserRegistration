using Core.Common.Contracts.Mail;
using Core.Common.Helpers;
using Core.Common.Mail;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;
using UserRegistration.Data.Contracts;
using UserRegistration.Data.Sql;
using UserRegistration.Data.Sql.Repositories;
using UserRegistration.Models;
using UserRegistration.Models.Settings;
using UserRegistration.Service.Contracts;

namespace UserRegistration.Service.Tests
{
    [TestClass]
    public class AcountServiceTests
    {

        [TestMethod]
        public async Task UserFromRegistrationToLoginTest()
        {

            string plainTextPassword = "somePassword";
            string email = "john.doe@mail.com";
            string username = "johndoe";


            var mockSecuritySettings = new Mock<IOptions<SecuritySettings>>();
            mockSecuritySettings.Setup(s => s.Value).Returns(new SecuritySettings
            {
                SecretKey = "superSecretKey",
                DevMode = true,
                SaltValidityInSeconds = 300,
                SessionValidityInSeconds = 60,
                VerificationLimitPerDay = 2
            });

            string verificationCode = "123456";
            var mockHelperService = new Mock<IHelperService>();
            mockHelperService.Setup(s => s.RandomString(6)).Returns(verificationCode);

            var mockMailService = new Mock<IMailService>();
            var mockHttpContext = new Mock<IHttpContextAccessor>();
            mockHttpContext.Setup(s => s.HttpContext.Session.Set("", null));

            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase("UserRegistration").Options;

            using (var dbContext = new DatabaseContext(options))
            {
                var userRepository = new UserRepository(dbContext);
                var emailVerificationRepository = new EmailVerificationRepository(dbContext);

                var authService = new AuthService(userRepository, mockSecuritySettings.Object, mockHelperService.Object, emailVerificationRepository, mockMailService.Object);
                // Mock client for email verification
                var verify = new VerificationRequest();
                verify.Command = "emailVerification";
                verify.Email = email;
                verify.Username = username;
                var verificationResponse = await authService.SendVerificationCode(verify);

                // Assert
                Assert.IsTrue(verificationResponse.Success);

                // Mock client request for registration         
                var userRegistrationRequest = new UserRegistrationRequest();
                userRegistrationRequest.Command = "register";
                userRegistrationRequest.Username = username;
                userRegistrationRequest.Email = email;
                userRegistrationRequest.Password = SecurityHelper.ComputeHash(plainTextPassword, userRegistrationRequest.Username);
                userRegistrationRequest.Password2 = SecurityHelper.ComputeHash(plainTextPassword, userRegistrationRequest.Email);
                userRegistrationRequest.VerificationCode = verificationCode;

                var helperService = new HelperService();
                authService = new AuthService(userRepository, mockSecuritySettings.Object, helperService, emailVerificationRepository, mockMailService.Object);
                var registerResponse = await authService.Register(userRegistrationRequest);

                // Assert
                Assert.IsTrue(registerResponse.Success);


                // Mock client request for getting Salt
                var loginSaltRequest = new LoginSaltRequest();
                loginSaltRequest.Command = "loginSalt";
                loginSaltRequest.Username = username;

                var loginService = new LoginService(userRepository, mockSecuritySettings.Object, mockHttpContext.Object, emailVerificationRepository, new HelperService());
                var saltResponse = await loginService.GenerateSalt(loginSaltRequest);

                var hashedPassword = SecurityHelper.ComputeHash(plainTextPassword, loginSaltRequest.Username);
                var hmac = SecurityHelper.ComputeHash(mockSecuritySettings.Object.Value.SecretKey, hashedPassword);
                var challenge = SecurityHelper.ComputeHash(saltResponse.Salt, hmac);

                var loginResponse = await loginService.Login(new LoginRequest
                {
                    Command = "login",
                    Username = loginSaltRequest.Username,
                    Challenge = challenge
                });

                Assert.IsTrue(!string.IsNullOrEmpty(loginResponse.SessionID));


            }

        }
    }
}
