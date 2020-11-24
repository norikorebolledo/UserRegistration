
using Core.Common.Model;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace UserRegistration.Models
{
    public class UserRegistrationRequest : BaseModel<UserRegistrationRequest>
    {
        public string Command { get; set; }
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string Password { get; set; }
        public string Password2 { get; set; }
        public string Email { get; set; }
        public string VerificationCode { get; set; }

        protected override AbstractValidator<UserRegistrationRequest> GetValidator()
        {
            return new Validator();
        }

        public class Validator : AbstractValidator<UserRegistrationRequest>
        {
            public Validator()
            {
                RuleFor(c => c.Username).NotEmpty().NotNull();
                RuleFor(c => c.Email).NotEmpty().NotNull();
                RuleFor(c => c.Password).NotEmpty().NotNull();
                RuleFor(c => c.VerificationCode).NotEmpty().NotNull();
            }
        }
    }
}
