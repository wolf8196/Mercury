using System;

namespace Mercury.Validation
{
    public class ModelValidationException : Exception
    {
        public ModelValidationException()
            : base("Provided model is invalid")
        {
        }
    }
}