using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UserRegistration.Models;
using UserRegistration.Service.Contracts;

namespace UserRegistration.Api.Areas.Backend.Apis
{
  
    [Route("backend/[controller]")]
    [ApiController]
    public class BackendController : ControllerBase
    {
        private readonly ILoginService _loginService;
        private readonly IAuthService _authService;

        public BackendController(ILoginService loginService,
            IAuthService authService)
        {
            _loginService = loginService;
            _authService = authService;
        }

        [HttpPost("checkUsername")]
        public async Task<IActionResult> CheckUsername(CheckUsernameAvailabilityRequest model)
        {
            return Ok(await _authService.CheckUsername(model));
        }

        [HttpPost("checkEmail")]
        public async Task<IActionResult> CheckEmail(CheckEmailAvailabilityRequest model)
        {
            return Ok(await _authService.CheckEmail(model));
        }


        [HttpPost("loginSalt")]
        public async Task<IActionResult> GenerateSalt(LoginSaltRequest model)
        {
            return Ok(await _loginService.GenerateSalt(model));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest model)
        {
            return Ok(await _loginService.Login(model));
        }

        [HttpPost("emailVerification")]
        public async Task<IActionResult> EmailVerication(VerificationRequest model)
        {
            return Ok(await _authService.SendVerificationCode(model));
        }


    }
}
