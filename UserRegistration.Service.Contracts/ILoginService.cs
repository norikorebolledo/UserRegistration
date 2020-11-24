
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UserRegistration.Models;

namespace UserRegistration.Service.Contracts
{
    public interface ILoginService
    {
        Task<LoginSaltResponse> GenerateSalt(LoginSaltRequest model);
        Task<LoginResponse> Login(LoginRequest model);
    


    }
}
