using Core.Common.Model;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace UserRegistration.Models
{
    public class VerifyCodeRequest : BaseModel<VerifyCodeRequest>
    {
        public string Command { get; set; }
        public string Email { get; set; }
        public string Code { get; set; }

        protected override AbstractValidator<VerifyCodeRequest> GetValidator()
        {
            return new Validator();
        }

        public class Validator : AbstractValidator<VerifyCodeRequest>
        {
            public Validator()
            {
                RuleFor(c => c.Email).NotNull();
                RuleFor(c => c.Code).NotNull();
            }
        }
    }
}
