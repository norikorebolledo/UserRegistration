using Core.Common.Model;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace UserRegistration.Models
{
    public class LoginRequest : BaseModel<LoginRequest>
    {
        public string Command { get; set; }
        public string Username { get; set; }
        public string Challenge { get; set; }
        public string Password { get; set; }

        protected override AbstractValidator<LoginRequest> GetValidator()
        {
            return new Validator();
        }

        public class Validator : AbstractValidator<LoginRequest>
        {
            public Validator()
            {
                RuleFor(c => c.Username).NotNull();
                RuleFor(c => c.Challenge).NotNull();
            }
        }
    }
}
