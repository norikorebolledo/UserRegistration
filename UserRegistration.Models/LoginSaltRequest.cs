using Core.Common.Model;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace UserRegistration.Models
{
    public class LoginSaltRequest : BaseModel<LoginSaltRequest>
    {
        public string Command { get; set; }
        public string Username { get; set; }

        protected override AbstractValidator<LoginSaltRequest> GetValidator()
        {
            return new Validator();
        }

        public class Validator : AbstractValidator<LoginSaltRequest>
        {
            public Validator()
            {
                RuleFor(c => c.Username).NotEmpty().NotNull();
            }
        }
    }
}
