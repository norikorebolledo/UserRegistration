using Core.Common.Helpers;
using Core.Common.Mail;
using Core.Data.MongoDb;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;
using UserRegistration.Data;
using UserRegistration.Data.Contracts;
using UserRegistration.Models;
using UserRegistration.Models.Settings;
using UserRegistration.Service.Contracts;

namespace UserRegistration.Service.Tests
{
    [TestClass]
    public class AcountServiceTests
    {

        const string ConnectionString = "mongodb://localhost:27017";
        const string DatabaseName = "userRegistration";

        [TestMethod]
        public async Task UserRegistrationTest()
        {
            // Arrange
            string plainTextPassword = "somePassword";
            string email = "john.doe@mail.com";
            string username = "johndoe";


            var mockSecuritySettings = new Mock<IOptions<SecuritySettings>>();
            mockSecuritySettings.Setup(s => s.Value).Returns(new SecuritySettings
            {
                SecretKey = "superSecretKey"
            });

            string verificationCode = "123456";
            var mockHelperService = new Mock<IHelperService>();
            mockHelperService.Setup(s => s.RandomString(6)).Returns(verificationCode);

            var mockMailService = new Mock<IMailService>();
            var mockHttpContext = new Mock<IHttpContextAccessor>();

            // Act
            var userRepository = new UserRepository(new MongoDbSettings { ConnectionString = ConnectionString, DatabaseName = DatabaseName });
            var emailVerificationRepository = new EmailVerificationRepository(new MongoDbSettings { ConnectionString = ConnectionString, DatabaseName = DatabaseName });
            var service = new AccountService(userRepository, mockSecuritySettings.Object, mockMailService.Object, mockHttpContext.Object, emailVerificationRepository, mockHelperService.Object);

            // Get email verification
            var verify = new VerificationRequest();
            verify.Command = "emailVerification";
            verify.Email = email;
            verify.Username = username;
            var verificationResponse = await service.SendVerificationCode(verify);

            // Mock client request            
            var userRegistrationRequest = new UserRegistrationRequest();
            userRegistrationRequest.Command = "register";
            userRegistrationRequest.Username = username;
            userRegistrationRequest.Email = email;
            userRegistrationRequest.Password = SecurityHelper.ComputeHash(plainTextPassword, userRegistrationRequest.Username);
            userRegistrationRequest.Password2 = SecurityHelper.ComputeHash(plainTextPassword, userRegistrationRequest.Email);
            userRegistrationRequest.VerificationCode = verificationCode;

            // Act
            var response = await service.Register(userRegistrationRequest);

            // Assert
            Assert.IsTrue(response.Success);

        }

        [TestMethod]
        public async Task AuthenticationTest()
        {
            // Arrange
            string plainTextPassword = "somePassword";

            // Mock client request
            var loginSaltRequest = new LoginSaltRequest();
            loginSaltRequest.Command = "loginSalt";
            loginSaltRequest.Username = "johndoe";

            // Act
            var mockUserRepository = new Mock<IUserRepository>();
            var returnUser = new Entities.User();
            returnUser = null;
            mockUserRepository.Setup(s => s.FindOneAsync(f => f.Username == loginSaltRequest.Username)).ReturnsAsync(returnUser);

            var mockSecuritySettings = new Mock<IOptions<SecuritySettings>>();
            mockSecuritySettings.Setup(s => s.Value).Returns(new SecuritySettings
            {
                SecretKey = "superSecretKey",
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

            // Act
            var userRepository = new UserRepository(new MongoDbSettings { ConnectionString = ConnectionString, DatabaseName = DatabaseName });
            var emailVerificationRepository = new EmailVerificationRepository(new MongoDbSettings { ConnectionString = ConnectionString, DatabaseName = DatabaseName });
            var service = new AccountService(userRepository, mockSecuritySettings.Object, mockMailService.Object, mockHttpContext.Object, emailVerificationRepository, mockHelperService.Object);
            var saltResponse = await service.GenerateSalt(loginSaltRequest);


            // Mock client request
            var hashedPassword = SecurityHelper.ComputeHash(plainTextPassword, loginSaltRequest.Username);
            var hmac = SecurityHelper.ComputeHash(mockSecuritySettings.Object.Value.SecretKey, hashedPassword);
            var challenge = SecurityHelper.ComputeHash(saltResponse.Salt, hmac);

            var loginResponse = await service.Login(new LoginRequest
            {
                Command = "login",
                Username = loginSaltRequest.Username,
                Challenge = challenge
            });

            // Assert
            Assert.IsTrue(!string.IsNullOrEmpty(loginResponse.SessionID));
        }
    }
}
