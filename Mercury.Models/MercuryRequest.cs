using System.Collections.Generic;
using System.Dynamic;

namespace Mercury.Models
{
    public class MercuryRequest
    {
        public string TemplateKey { get; set; }

        public IEnumerable<string> Tos { get; set; }

        public IEnumerable<string> Ccs { get; set; }

        public IEnumerable<string> Bccs { get; set; }

        public ExpandoObject Payload { get; set; }
    }
}