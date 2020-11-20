using Core.Common.Helpers;
using Core.Common.Mail;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UserRegistration.Data.Contracts;
using UserRegistration.Entities;
using UserRegistration.Models;
using UserRegistration.Models.Settings;
using UserRegistration.Service.Contracts;

namespace UserRegistration.Service
{
    public class AccountService : IAccountService
    {
        private readonly IUserRepository _userRepository;
        private readonly SecuritySettings _securitySettings;
        private readonly IMailService _mailService;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IEmailVerificationRepository _emailVerificationRepository;
        private readonly IHelperService _helperService;

        public AccountService(IUserRepository userRepository,
            IOptions<SecuritySettings> securitySettings,
            IMailService mailService, IHttpContextAccessor httpContext,
            IEmailVerificationRepository emailVerificationRepository,
            IHelperService helperService)
        {
            _userRepository = userRepository;
            _securitySettings = securitySettings.Value;
            _mailService = mailService;
            _httpContext = httpContext;
            _emailVerificationRepository = emailVerificationRepository;
            _helperService = helperService;
        }


        public async Task<UserRegistrationResponse> Register(UserRegistrationRequest model)
        {


            bool isValid = true;
            bool isSuccess = false;
            string message = string.Empty;

            // Validation
            if (model.IsInvalid())
            {
                message = string.Join(',', model.GetErrors());
                isValid = false;
            }

            if (isValid)
            {
                var checkUser = await CheckUsername(new CheckUsernameAvailabilityRequest { Username = model.Username });
                isValid = checkUser.Available;
            }


            if (isValid)
            {
                var checkEmail = await CheckEmail(new CheckEmailAvailabilityRequest { Email = model.Email });
                isValid = checkEmail.Available;
            }

            if (isValid)
            {
                var verify = await Verify(new VerifyCodeRequest { Email = model.Email, Code = model.VerificationCode });
                isValid = verify.Success;
            }


            if (isValid)
            {
                var hmacForSaving = SecurityHelper.ComputeHash(_securitySettings.SecretKey, model.Password);
                var entity = new User
                {
                    Username = model.Username,
                    Email = model.Email,
                    DisplayName = model.DisplayName,
                    Password = hmacForSaving
                };
                await _userRepository.InsertOneAsync(entity);
                isSuccess = true;
            }

            return new UserRegistrationResponse
            {
                Command = "register",
                Username = model.Username,
                Success = isSuccess,
                Message = message
            };
        }

        public async Task<LoginSaltResponse> GenerateSalt(LoginSaltRequest model)
        {

            var emailVerification = _emailVerificationRepository.FindOneAsync(f => f.Username == model.Username && f.IsVerified);
            var user = await _userRepository.FindOneAsync(f => f.Username == model.Username);

            if (emailVerification != null && user != null)
            {
                user.Salt = SecurityHelper.RandomString(64);
                user.SaltValidity = _securitySettings.SaltValidityInSeconds;
                user.SaltGenerationDate = DateTime.Now;

                await _userRepository.ReplaceOneAsync(user);

                return new LoginSaltResponse
                {
                    Command = "loginSalt",
                    Username = user.Username,
                    Validity = user.SaltValidity,
                    Salt = user.Salt
                };
            }

            return new LoginSaltResponse
            {
                Success = false,
                Message = "User not found"
            };

        }

        public async Task<LoginResponse> Login(LoginRequest model)
        {
            bool isValid = true;
            string message = string.Empty;
            if (model.IsInvalid())
            {
                message = string.Join(',', model.GetErrors());
                isValid = false;
            }

            if (isValid)
            {
                var user = await _userRepository.FindOneAsync(f => f.Username == model.Username);

                if (user != null)
                {
                    if (user.SaltGenerationDate.HasValue && DateTime.Now > user.SaltGenerationDate.Value.ToLocalTime().AddSeconds(user.SaltValidity))
                    {
                        return new LoginResponse
                        {
                            Command = "login",
                            Success = false,
                            Message = "Salt expired"
                        };
                    }

                    var challenge = SecurityHelper.ComputeHash(user.Salt, user.Password);

                    if (challenge != model.Challenge)
                    {
                        return new LoginResponse
                        {
                            Command = "login",
                            Success = false,
                            Message = "Challenge is not valid"
                        };
                    }

                    string sessionId = Guid.NewGuid().ToString();
                    _httpContext.HttpContext.Session.Set("sessionID", Encoding.UTF8.GetBytes(sessionId));
                    _httpContext.HttpContext.Session.Set("sessionStarted", Encoding.UTF8.GetBytes(DateTime.Now.ToString()));

                    return new LoginResponse
                    {
                        Command = "login",
                        SessionID = sessionId,
                        Success = true,
                        UserID = user.Id.ToString(),
                        Username = user.Username,
                        Validity = _securitySettings.SessionValidityInSeconds
                    };

                }
            }

            return new LoginResponse
            {
                Command = "login",
                Success = false,
                Message = message
            };
        }

        public async Task<VerificationResponse> SendVerificationCode(VerificationRequest model)
        {
            bool isSuccess = false;
            bool isValid = true;

            string message = string.Empty;
            if (model.IsInvalid())
            {
                message = string.Join(',', model.GetErrors());
                isValid = false;
            }


            var emailVerification = await _emailVerificationRepository.FindOneAsync(f => f.Email == model.Email && f.Username == model.Username);

            if (emailVerification == null)
            {
                emailVerification = new EmailVerification();
                emailVerification.Username = model.Username;
                emailVerification.Email = model.Email;
                await _emailVerificationRepository.InsertOneAsync(emailVerification);
            }

            // Check verification limit per day
            if (emailVerification.CodeSentDate.HasValue && emailVerification.CodeSentDate.Value.Date == DateTime.Now.Date && emailVerification.Counter >= _securitySettings.VerificationLimitPerDay)
            {
                isValid = false;
            }

            if (isValid)
            {
                try
                {

                    string code = _helperService.RandomString(6);
                    string body = $"Your verification code is {code}";

                    await _mailService.SendEmailAsync(new MailRequest
                    {
                        Subject = "Verify Account",
                        ToEmail = emailVerification.Email,
                        Body = body
                    });

                    emailVerification.Code = code;
                    if (emailVerification.Counter > 0)
                    {
                        ++emailVerification.Counter;
                    }
                    else
                    {
                        emailVerification.Counter = 1;
                        emailVerification.CodeSentDate = DateTime.Now;
                    }

                    _emailVerificationRepository.ReplaceOne(emailVerification);

                    isSuccess = true;
                }
                catch (Exception)
                {
                    // Log
                }
            }

            return new VerificationResponse
            {
                Command = "verification",
                Success = isSuccess,
                Message = message
            };
        }

        public async Task<VerifyCodeResponse> Verify(VerifyCodeRequest model)
        {
            bool isSuccess = false;
            var emailVerification = await _emailVerificationRepository.FindOneAsync(f => f.Email == model.Email);

            if (emailVerification != null && emailVerification.Code == model.Code)
            {
                emailVerification.IsVerified = true;
                _emailVerificationRepository.ReplaceOne(emailVerification);
                isSuccess = true;
            }

            return new VerifyCodeResponse
            {
                Command = "verifyCode",
                Success = isSuccess
            };
        }

        public async Task<CheckUsernameAvailabilityResponse> CheckUsername(CheckUsernameAvailabilityRequest model)
        {
            var user = await _userRepository.FindOneAsync(f => f.Username == model.Username);

            return new CheckUsernameAvailabilityResponse
            {
                Command = "checkUsername",
                Username = model.Username,
                Available = user == null
            };
        }

        public async Task<CheckEmailAvailabilityResponse> CheckEmail(CheckEmailAvailabilityRequest model)
        {
            var user = await _userRepository.FindOneAsync(f => f.Email == model.Email);

            return new CheckEmailAvailabilityResponse
            {
                Command = "checkUsername",
                Email = model.Email,
                Available = user == null
            };
        }
    }
}
