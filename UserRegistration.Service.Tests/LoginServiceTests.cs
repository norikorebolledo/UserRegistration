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
    public class LoginServiceTests
    {

        [TestMethod]
        public async Task GenerateSaltUserNotFoundTest()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase("UserRegistration").Options;

            using (var dbContext = new DatabaseContext(options))
            {
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

                var mockHttpContext = new Mock<IHttpContextAccessor>();
                mockHttpContext.Setup(s => s.HttpContext.Session.Set("", null));

                var loginService = new LoginService(userRepository, mockSecuritySettings.Object, mockHttpContext.Object, emailVerificationRepository, new HelperService(), new AppDateTime());

                // Act
                var saltResponse = await loginService.GenerateSalt(new LoginSaltRequest
                {
                    Username = "johndoe"
                });

                // Assert
                Assert.IsTrue(!saltResponse.Success);
            }
        }

        [TestMethod]
        public async Task GenerateSaltUserSuccessTest()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase("UserRegistration").Options;

            using (var dbContext = new DatabaseContext(options))
            {
                dbContext.EmailVerifications.Add(new Entities.EmailVerification { Username = "johndoe", IsVerified = true });
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

                var mockHttpContext = new Mock<IHttpContextAccessor>();
                mockHttpContext.Setup(s => s.HttpContext.Session.Set("", null));


                var loginService = new LoginService(userRepository, mockSecuritySettings.Object, mockHttpContext.Object, emailVerificationRepository, new HelperService(), new AppDateTime());

                // Act
                var saltResponse = await loginService.GenerateSalt(new LoginSaltRequest
                {
                    Username = "johndoe"
                });

                // Assert
                Assert.IsTrue(saltResponse.Success);
            }
        }

        [TestMethod]
        public async Task LoginUserNotFoundTest()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase("UserRegistration").Options;

            using (var dbContext = new DatabaseContext(options))
            {
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

                var mockHttpContext = new Mock<IHttpContextAccessor>();
                mockHttpContext.Setup(s => s.HttpContext.Session.Set("", null));


                var loginService = new LoginService(userRepository, mockSecuritySettings.Object, mockHttpContext.Object, emailVerificationRepository, new HelperService(), new AppDateTime());

                // Act
                var saltResponse = await loginService.Login(new LoginRequest
                {
                    Username = "johndoe",
                    Challenge = "challenge"
                });

                // Assert
                Assert.IsTrue(!saltResponse.Success);
            }
        }

        [TestMethod]
        public async Task LoginSaltExpiredTest()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase("UserRegistration").Options;

            using (var dbContext = new DatabaseContext(options))
            {
                var saltGenerationDate = new DateTime(2020, 11, 25, 12, 0, 0);
                dbContext.Users.Add(new Entities.User { Id = "1", Username = "johndoe", SaltGenerationDate = saltGenerationDate });
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

                var mockDateTime = new Mock<IDateTime>();
                mockDateTime.Setup(s => s.UtcNow).Returns(saltGenerationDate.AddSeconds(301)); // Added 301 seconds to make it expired based on the salt validity

                var mockHttpContext = new Mock<IHttpContextAccessor>();
                mockHttpContext.Setup(s => s.HttpContext.Session.Set("", null));
                var loginService = new LoginService(userRepository, mockSecuritySettings.Object, mockHttpContext.Object, emailVerificationRepository, new HelperService(), mockDateTime.Object);

                // Act
                var saltResponse = await loginService.Login(new LoginRequest
                {
                    Username = "johndoe",
                    Challenge = "challenge"
                });

                // Assert
                Assert.IsTrue(!saltResponse.Success);
            }
        }

        [TestMethod]
        public async Task LoginSuccessTest()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase("UserRegistration").Options;

            using (var dbContext = new DatabaseContext(options))
            {
                var saltGenerationDate = new DateTime(2020, 11, 25, 12, 0, 0);
                dbContext.Users.Add(new Entities.User { Id = "1", Username = "johndoe", SaltGenerationDate = saltGenerationDate, Salt = "salt", Password = "password" });
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

                var mockDateTime = new Mock<IDateTime>();
                mockDateTime.Setup(s => s.UtcNow).Returns(saltGenerationDate.AddSeconds(300));

                var mockHelperService = new Mock<IHelperService>();
                mockHelperService.Setup(s => s.ComputeHash("salt", "password")).Returns("challenge");

                var mockHttpContext = new Mock<IHttpContextAccessor>();
                mockHttpContext.Setup(s => s.HttpContext.Session.Set("", null));
                var loginService = new LoginService(userRepository, mockSecuritySettings.Object, mockHttpContext.Object, emailVerificationRepository, mockHelperService.Object, mockDateTime.Object);

                // Act
                var saltResponse = await loginService.Login(new LoginRequest
                {
                    Username = "johndoe",
                    Challenge = "challenge"
                });

                // Assert
                Assert.IsTrue(saltResponse.Success);
            }
        }

    }
}
