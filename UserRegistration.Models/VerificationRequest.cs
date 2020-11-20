using Core.Common.Model;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace UserRegistration.Models
{
    public class VerificationRequest : BaseModel<VerificationRequest>
    {
        public string Command { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }

        protected override AbstractValidator<VerificationRequest> GetValidator()
        {
            return new Validator();
        }

        public class Validator : AbstractValidator<VerificationRequest>
        {
            public Validator()
            {
                RuleFor(c => c.Email).NotNull();
                RuleFor(c => c.Username).NotNull();
            }
        }
    }
}
