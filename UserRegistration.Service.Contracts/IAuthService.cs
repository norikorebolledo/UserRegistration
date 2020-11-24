using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UserRegistration.Models;

namespace UserRegistration.Service.Contracts
{
    public interface IAuthService
    {
        Task<UserRegistrationResponse> Register(UserRegistrationRequest model);
        Task<VerificationResponse> SendVerificationCode(VerificationRequest model);
        Task<VerifyCodeResponse> Verify(VerifyCodeRequest model);
        Task<CheckUsernameAvailabilityResponse> CheckUsername(CheckUsernameAvailabilityRequest model);
        Task<CheckEmailAvailabilityResponse> CheckEmail(CheckEmailAvailabilityRequest model);

    }
}
