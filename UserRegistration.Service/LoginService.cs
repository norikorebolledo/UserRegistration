using Core.Common.Contracts.Date;
using Core.Common.Contracts.Mail;
using Core.Common.Contracts.Session;
using Core.Common.Helpers;
using Core.Common.Mail;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Text;
using System.Threading.Tasks;
using UserRegistration.Data.Contracts;
using UserRegistration.Entities;
using UserRegistration.Models;
using UserRegistration.Models.Settings;
using UserRegistration.Service.Contracts;

namespace UserRegistration.Service
{
    public class LoginService : ILoginService
    {
        private readonly IUserRepository _userRepository;
        private readonly SecuritySettings _securitySettings;
        private readonly IEmailVerificationRepository _emailVerificationRepository;
        private readonly IHelperService _helperService;
        private readonly IDateTime _dateTime;
        private readonly ISessionService _sessionService;

        public LoginService(IUserRepository userRepository,
            IOptions<SecuritySettings> securitySettings,
            IEmailVerificationRepository emailVerificationRepository,
            IHelperService helperService,
            IDateTime dateTime,
            ISessionService sessionService)
        {
            _userRepository = userRepository;
            _securitySettings = securitySettings.Value;
            _emailVerificationRepository = emailVerificationRepository;
            _helperService = helperService;
            _dateTime = dateTime;
            _sessionService = sessionService;
        }


        public async Task<LoginSaltResponse> GenerateSalt(LoginSaltRequest model)
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
                var emailVerification = await _emailVerificationRepository.FindAsync(f => f.Username == model.Username && f.IsVerified);
                var user = await _userRepository.FindAsync(f => f.Username == model.Username);

                if (emailVerification != null && user != null)
                {
                    user.Salt = _helperService.RandomString(64);
                    user.SaltValidity = _securitySettings.SaltValidityInSeconds;
                    user.SaltGenerationDate = DateTime.Now;

                    await _userRepository.UpdateAsync(user);

                    return new LoginSaltResponse
                    {
                        Command = "loginSalt",
                        Username = user.Username,
                        Validity = user.SaltValidity,
                        Salt = user.Salt,
                        Success = true
                    };
                }
                else
                {
                    message = "User not found";
                }
            }

            return new LoginSaltResponse
            {
                Success = false,
                Message = message
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
                var user = await _userRepository.FindAsync(f => f.Username == model.Username);

                if (user != null)
                {
                    if (user.SaltGenerationDate.HasValue && _dateTime.UtcNow > user.SaltGenerationDate.Value.AddSeconds(_securitySettings.SaltValidityInSeconds))
                    {
                        return new LoginResponse
                        {
                            Command = "login",
                            Success = false,
                            Message = "Salt expired"
                        };
                    }

                    var challenge = _helperService.ComputeHash(user.Salt, user.Password);

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
                    await _sessionService.SetAsync(sessionId, user.Username, _securitySettings.SessionValidityInSeconds);

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
                else
                {
                    message = "User not found";
                }
            }

            return new LoginResponse
            {
                Command = "login",
                Success = false,
                Message = message
            };
        }


    }
}
