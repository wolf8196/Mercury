﻿using System.Dynamic;
using FluentValidation;
using Mercury.Models;

namespace Mercury.Core.Validation
{
    public class MercuryRequestValidator : AbstractValidator<MercuryRequest<ExpandoObject>>
    {
        public MercuryRequestValidator()
        {
            RuleFor(x => x.TemplateKey).NotEmpty();
            RuleFor(x => x.Tos).NotEmpty();
            RuleFor(x => x.Payload).NotEmpty();
        }
    }
}