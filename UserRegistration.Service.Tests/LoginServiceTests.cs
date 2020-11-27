using Core.Common.Contracts.Date;
using Core.Common.Contracts.Session;
using Core.Common.Date;
using Core.Common.Session;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading.Tasks;
using UserRegistration.Data.Sql;
using UserRegistration.Data.Sql.Repositories;
using UserRegistration.Models;
using UserRegistration.Models.Settings;
using UserRegistration.Service.Contracts;
using UserRegistration.Service.Tests.Mocks;

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
                .UseInMemoryDatabase("UserRegistration1").Options;

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

                var mockSessionService = new Mock<ISessionService>();

                var loginService = new LoginService(userRepository, mockSecuritySettings.Object, emailVerificationRepository, new HelperService(), new AppDateTime(), mockSessionService.Object);

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
                .UseInMemoryDatabase("UserRegistration2").Options;

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

                var mockSessionService = new Mock<ISessionService>();

                var loginService = new LoginService(userRepository, mockSecuritySettings.Object, emailVerificationRepository, new HelperService(), new AppDateTime(), mockSessionService.Object);

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
                .UseInMemoryDatabase("UserRegistration3").Options;

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

                var mockSessionService = new Mock<ISessionService>();

                var loginService = new LoginService(userRepository, mockSecuritySettings.Object, emailVerificationRepository, new HelperService(), new AppDateTime(), mockSessionService.Object);

                // Act
                var saltResponse = await loginService.Login(new LoginRequest
                {
                    UsernameOrEmail = "johndoe",
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
                .UseInMemoryDatabase("UserRegistration4").Options;

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

                var mockSessionService = new Mock<ISessionService>();

                var loginService = new LoginService(userRepository, mockSecuritySettings.Object, emailVerificationRepository, new HelperService(), mockDateTime.Object, mockSessionService.Object);

                // Act
                var saltResponse = await loginService.Login(new LoginRequest
                {
                    UsernameOrEmail = "johndoe",
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
                .UseInMemoryDatabase("UserRegistration5").Options;

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

                var mockInMemoryCaching = new MockDistributedCache();
                var loginService = new LoginService(userRepository, mockSecuritySettings.Object, emailVerificationRepository, mockHelperService.Object, mockDateTime.Object, new SessionService(mockInMemoryCaching));

                // Act
                var saltResponse = await loginService.Login(new LoginRequest
                {
                    UsernameOrEmail = "johndoe",
                    Challenge = "challenge"
                });

                // Assert
                Assert.IsTrue(saltResponse.Success);
                var userSession = await mockInMemoryCaching.GetAsync(saltResponse.SessionID);

                // Verify if session created
                Assert.IsTrue(userSession != null);
            }
        }



    }
}
