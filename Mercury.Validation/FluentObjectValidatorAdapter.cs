using FluentObjectValidator;
using Mercury.Validation.Configurations;

namespace Mercury.Validation
{
    internal class FluentObjectValidatorAdapter : Validator
    {
        public FluentObjectValidatorAdapter()
        {
            AddConfiguration(new EmailRequestModelConfigurations());
        }
    }
}