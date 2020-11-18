namespace Mercury.Core.Emailers
{
    public enum EmailerTypes : byte
    {
        Unknown,
        Mock,
        SendGrid,
        Smtp
    }
}