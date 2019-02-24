namespace Mercury.Validation
{
    public interface IValidator
    {
        void Validate<T>(T model);
    }
}