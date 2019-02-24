using System;
using Mercury.Emailers.Enums;

namespace Mercury.Emailers.Exceptions
{
    public class EmailingException : Exception
    {
        public EmailingException(EmailerTypes type)
            : this("Failed to send an email", type)
        {
        }

        public EmailingException(string msg, EmailerTypes type)
            : base(msg)
        {
            EmailerType = type;
        }

        public EmailerTypes EmailerType { get; set; }
    }
}