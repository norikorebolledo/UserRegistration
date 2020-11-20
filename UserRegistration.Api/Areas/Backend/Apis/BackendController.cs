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
        private readonly IAccountService _accountService;

        public BackendController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("checkUsername")]
        public async Task<IActionResult> CheckUsername(CheckUsernameAvailabilityRequest model)
        {
            return Ok(await _accountService.CheckUsername(model));
        }

        [HttpPost("checkEmail")]
        public async Task<IActionResult> CheckEmail(CheckEmailAvailabilityRequest model)
        {
            return Ok(await _accountService.CheckEmail(model));
        }


        [HttpPost("loginSalt")]
        public async Task<IActionResult> GenerateSalt(LoginSaltRequest model)
        {
            return Ok(await _accountService.GenerateSalt(model));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest model)
        {
            return Ok(await _accountService.Login(model));
        }

        [HttpPost("emailVerification")]
        public async Task<IActionResult> EmailVerication(VerificationRequest model)
        {
            return Ok(await _accountService.SendVerificationCode(model));
        }


    }
}
