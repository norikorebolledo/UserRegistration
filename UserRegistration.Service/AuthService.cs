using Core.Common.Contracts.Date;
using Core.Common.Contracts.Mail;
using Core.Common.Helpers;
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
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly SecuritySettings _securitySettings;
        private readonly IHelperService _helperService;
        private readonly IEmailVerificationRepository _emailVerificationRepository;
        private readonly IMailService _mailService;
        private readonly IDateTime _dateTime;

        public AuthService(IUserRepository userRepository,
             IOptions<SecuritySettings> securitySettings,
             IHelperService helperService,
             IEmailVerificationRepository emailVerificationRepository,
             IMailService mailService,
             IDateTime dateTime)
        {
            _userRepository = userRepository;
            _securitySettings = securitySettings.Value;
            _helperService = helperService;
            _emailVerificationRepository = emailVerificationRepository;
            _mailService = mailService;
            _dateTime = dateTime;
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
                message = checkUser.Message;
                isValid = checkUser.Available;
            }


            if (isValid)
            {
                var checkEmail = await CheckEmail(new CheckEmailAvailabilityRequest { Email = model.Email });
                message = checkEmail.Message;
                isValid = checkEmail.Available;
            }

            if (isValid)
            {
                var verify = await Verify(new VerifyCodeRequest { Email = model.Email, Code = model.VerificationCode });
                message = verify.Message;
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
                await _userRepository.AddAsync(entity);
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

            var emailVerification = await _emailVerificationRepository.FindAsync(f => f.Email == model.Email);

            if (emailVerification == null)
            {
                emailVerification = new EmailVerification();
                emailVerification.Username = model.Username;
                emailVerification.Email = model.Email;
                await _emailVerificationRepository.AddAsync(emailVerification);
            }

            // Check verification limit per day
            if (emailVerification.CodeSentDate.HasValue && emailVerification.CodeSentDate.Value.Date == _dateTime.UtcNow.Date && emailVerification.Counter >= _securitySettings.VerificationLimitPerDay)
            {
                message = "You reached the limit of verifying email";
                isValid = false;
            }

            if (isValid)
            {
                try
                {

                    string code = _helperService.RandomString(6);
                    string body = $"Your verification code is {code}";

                    if (_securitySettings.DevMode)
                    {
                        message = body;

                    }
                    else
                    {
                        await _mailService.SendEmailAsync(new MailRequest
                        {
                            Subject = "Verify Account",
                            ToEmail = emailVerification.Email,
                            Body = body
                        });
                    }

                    emailVerification.Code = code;
                    if (emailVerification.Counter > 0 && emailVerification.CodeSentDate.Value.Date == _dateTime.UtcNow.Date)
                    {
                        emailVerification.Counter++;
                    }
                    else
                    {
                        emailVerification.Counter = 1;
                        emailVerification.CodeSentDate = _dateTime.UtcNow;
                    }

                    await _emailVerificationRepository.UpdateAsync(emailVerification);

                    isSuccess = true;
                }
                catch (Exception ex)
                {
                    message = "Something wrong while sending email";
                    // Log
                }
            }

            return new VerificationResponse
            {
                Command = "emailVerification",
                Success = isSuccess,
                Message = message
            };
        }
        public async Task<VerifyCodeResponse> Verify(VerifyCodeRequest model)
        {
            bool isSuccess = false;
            string message = string.Empty;
            var emailVerification = await _emailVerificationRepository.FindAsync(f => f.Email == model.Email);

            if (emailVerification != null && emailVerification.Code == model.Code)
            {
                emailVerification.IsVerified = true;
                await _emailVerificationRepository.UpdateAsync(emailVerification);
                isSuccess = true;
            }
            else
            {
                message = "Verification code not matched";
            }

            return new VerifyCodeResponse
            {
                Command = "verifyCode",
                Success = isSuccess,
                Message = message
            };
        }
        public async Task<CheckUsernameAvailabilityResponse> CheckUsername(CheckUsernameAvailabilityRequest model)
        {
            var user = await _userRepository.FindAsync(f => f.Username == model.Username);

            return new CheckUsernameAvailabilityResponse
            {
                Command = "checkUsername",
                Username = model.Username,
                Available = user == null,
                Message = user != null ? "User already taken" : string.Empty
            };
        }
        public async Task<CheckEmailAvailabilityResponse> CheckEmail(CheckEmailAvailabilityRequest model)
        {
            var user = await _userRepository.FindAsync(f => f.Email == model.Email);

            return new CheckEmailAvailabilityResponse
            {
                Command = "checkEmail",
                Email = model.Email,
                Available = user == null,
                Message = user != null ? "Email already taken" : string.Empty
            };
        }

    }
}
