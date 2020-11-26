using Core.Common.Contracts.Date;
using Core.Common.Contracts.Mail;
using Core.Common.Date;
using Core.Common.Helpers;
using Core.Common.Mail;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
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
    public class AuthServiceTests
    {

        [TestMethod]
        public async Task RegisterUsernameAlreadyTakenTest()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase("UserRegistration1").Options;

            using (var dbContext = new DatabaseContext(options))
            {
                dbContext.Users.Add(new Entities.User { Id = "1", Username = "johndoe" });
                dbContext.SaveChanges();

                var userRepository = new UserRepository(dbContext);
                var emailVerificationRepository = new EmailVerificationRepository(dbContext);

                var mockSecuritySettings = new Mock<IOptions<SecuritySettings>>();
                mockSecuritySettings.Setup(s => s.Value).Returns(new SecuritySettings
                {
                    SecretKey = "superSecretKey",
                    DevMode = true,
                    SaltValidityInSeconds = 300,
                    SessionValidityInSeconds = 60,
                    VerificationLimitPerDay = 2
                });

                var mockMailService = new Mock<IMailService>();

                var authService = new AuthService(userRepository, mockSecuritySettings.Object, new HelperService(), emailVerificationRepository, mockMailService.Object, new AppDateTime());

                // Act
                var registerResponse = await authService.Register(new UserRegistrationRequest
                {
                    Username = "johndoe",
                    Email = "johndoe@email.com",
                    Password = "password",
                    VerificationCode = "1111"
                });

                // Assert
                Assert.IsTrue(!registerResponse.Success);
            }
        }

        [TestMethod]
        public async Task RegisterEmailAlreadyTakenTest()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase("UserRegistration2").Options;

            using (var dbContext = new DatabaseContext(options))
            {
                dbContext.Users.Add(new Entities.User { Id = "1", Email = "johndoe@email.com" });
                dbContext.SaveChanges();

                var userRepository = new UserRepository(dbContext);
                var emailVerificationRepository = new EmailVerificationRepository(dbContext);

                var mockSecuritySettings = new Mock<IOptions<SecuritySettings>>();
                mockSecuritySettings.Setup(s => s.Value).Returns(new SecuritySettings
                {
                    SecretKey = "superSecretKey",
                    DevMode = true,
                    SaltValidityInSeconds = 300,
                    SessionValidityInSeconds = 60,
                    VerificationLimitPerDay = 2
                });

                var mockMailService = new Mock<IMailService>();

                var authService = new AuthService(userRepository, mockSecuritySettings.Object, new HelperService(), emailVerificationRepository, mockMailService.Object, new AppDateTime());

                // Act
                var registerResponse = await authService.Register(new UserRegistrationRequest
                {
                    Username = "johndoe",
                    Email = "johndoe@email.com",
                    Password = "password",
                    VerificationCode = "1111"
                });

                // Assert
                Assert.IsTrue(!registerResponse.Success);
            }
        }

        [TestMethod]
        public async Task RegisterVericationNotMatchedTest()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase("UserRegistration3").Options;

            using (var dbContext = new DatabaseContext(options))
            {
                dbContext.EmailVerifications.Add(new Entities.EmailVerification { Username = "johndoe", Code = "1111", Email = "johndoe@email.com" });
                dbContext.SaveChanges();

                var userRepository = new UserRepository(dbContext);
                var emailVerificationRepository = new EmailVerificationRepository(dbContext);

                var mockSecuritySettings = new Mock<IOptions<SecuritySettings>>();
                mockSecuritySettings.Setup(s => s.Value).Returns(new SecuritySettings
                {
                    SecretKey = "superSecretKey",
                    DevMode = true,
                    SaltValidityInSeconds = 300,
                    SessionValidityInSeconds = 60,
                    VerificationLimitPerDay = 2
                });

                var mockMailService = new Mock<IMailService>();

                var authService = new AuthService(userRepository, mockSecuritySettings.Object, new HelperService(), emailVerificationRepository, mockMailService.Object, new AppDateTime());

                // Act
                var registerResponse = await authService.Register(new UserRegistrationRequest
                {
                    Username = "johndoe",
                    Email = "johndoe@email.com",
                    Password = "password",
                    VerificationCode = "2222"
                });

                // Assert
                Assert.IsTrue(!registerResponse.Success);
            }
        }

        [TestMethod]
        public async Task SendVerificationReachedLimitTest()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase("UserRegistration4").Options;

            using (var dbContext = new DatabaseContext(options))
            {
                dbContext.EmailVerifications.Add(new Entities.EmailVerification { Username = "johndoe", Code = "1111", Email = "johndoe@email.com", CodeSentDate = new DateTime(2020, 11, 25, 12, 0, 0), Counter = 2 });
                dbContext.SaveChanges();

                var userRepository = new UserRepository(dbContext);
                var emailVerificationRepository = new EmailVerificationRepository(dbContext);

                var mockSecuritySettings = new Mock<IOptions<SecuritySettings>>();
                mockSecuritySettings.Setup(s => s.Value).Returns(new SecuritySettings
                {
                    SecretKey = "superSecretKey",
                    DevMode = true,
                    SaltValidityInSeconds = 300,
                    SessionValidityInSeconds = 60,
                    VerificationLimitPerDay = 2
                });

                var mockMailService = new Mock<IMailService>();
                var mockDateTime = new Mock<IDateTime>();
                mockDateTime.Setup(s => s.UtcNow).Returns(new DateTime(2020, 11, 25, 13, 0, 0));

                var authService = new AuthService(userRepository, mockSecuritySettings.Object, new HelperService(), emailVerificationRepository, mockMailService.Object, mockDateTime.Object);

                // Act
                var registerResponse = await authService.SendVerificationCode(new VerificationRequest { 
                    Username = "johndoe",
                    Email = "johndoe@email.com"
                });

                // Assert
                Assert.IsTrue(!registerResponse.Success);
            }
        }

        [TestMethod]
        public async Task SendVerificationCanSendOnTheNextDayTest()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase("UserRegistration5").Options;

            using (var dbContext = new DatabaseContext(options))
            {
                dbContext.EmailVerifications.Add(new Entities.EmailVerification { Username = "johndoe", Code = "1111", Email = "johndoe@email.com", CodeSentDate = new DateTime(2020, 11, 25, 12, 0, 0), Counter = 2 });
                dbContext.SaveChanges();

                var userRepository = new UserRepository(dbContext);
                var emailVerificationRepository = new EmailVerificationRepository(dbContext);

                var mockSecuritySettings = new Mock<IOptions<SecuritySettings>>();
                mockSecuritySettings.Setup(s => s.Value).Returns(new SecuritySettings
                {
                    SecretKey = "superSecretKey",
                    DevMode = true,
                    SaltValidityInSeconds = 300,
                    SessionValidityInSeconds = 60,
                    VerificationLimitPerDay = 2
                });

                var mockMailService = new Mock<IMailService>();
                var mockDateTime = new Mock<IDateTime>();
                mockDateTime.Setup(s => s.UtcNow).Returns(new DateTime(2020, 11, 26, 12, 0, 0));

                var authService = new AuthService(userRepository, mockSecuritySettings.Object, new HelperService(), emailVerificationRepository, mockMailService.Object, mockDateTime.Object);

                // Act
                var registerResponse = await authService.SendVerificationCode(new VerificationRequest
                {
                    Username = "johndoe",
                    Email = "johndoe@email.com"
                });

                // Assert
                Assert.IsTrue(registerResponse.Success);
            }
        }

    }
}
