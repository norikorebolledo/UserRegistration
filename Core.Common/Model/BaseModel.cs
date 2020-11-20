using FluentValidation;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Common.Model
{
    public abstract class BaseModel<TModel>
    {

        public BaseModel()
        {
            _validator = GetValidator();
        }

        IValidator _validator;

        protected abstract AbstractValidator<TModel> GetValidator();

        public bool IsInvalid()
        {
            var result = _validator.Validate(this);
            return result.Errors.Count > 0;
        }

        public IList<ValidationFailure> GetErrors()
        {
            var result = _validator.Validate(this);
            return result.Errors;
        }


    }
}
