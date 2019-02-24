using System.Collections.Generic;

namespace Mercury.Abstraction.Models
{
    public class EmailMessage
    {
        public EmailAddress From { get; set; }

        public IEnumerable<string> Tos { get; set; }

        public IEnumerable<string> Ccs { get; set; }

        public IEnumerable<string> Bccs { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }
    }
}