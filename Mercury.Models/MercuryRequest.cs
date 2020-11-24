using System.Collections.Generic;

namespace Mercury.Models
{
    public class MercuryRequest<TPayload>
        where TPayload : class
    {
        public string TemplateKey { get; set; }

        public IEnumerable<string> Tos { get; set; }

        public IEnumerable<string> Ccs { get; set; }

        public IEnumerable<string> Bccs { get; set; }

        public TPayload Payload { get; set; }
    }
}