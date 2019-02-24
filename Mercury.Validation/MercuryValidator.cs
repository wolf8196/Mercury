namespace Mercury.Validation
{
    public class MercuryValidator : IValidator
    {
        private readonly FluentObjectValidatorAdapter validator;

        public MercuryValidator()
        {
            validator = new FluentObjectValidatorAdapter();
        }

        public void Validate<T>(T model)
        {
            if (model == null || !validator.Validate(model))
            {
                throw new ModelValidationException();
            }
        }
    }
}