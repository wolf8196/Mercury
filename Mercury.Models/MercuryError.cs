using System.Collections.Generic;
using System.Linq;
using FluentResults;

namespace Mercury.Models
{
    public class MercuryError
    {
        public string Message { get; set; }

        public IDictionary<string, string> Metadata { get; set; }

        public IEnumerable<MercuryError> Reasons { get; set; }

        public static MercuryError Map(Error error)
        {
            return new MercuryError
            {
                Message = error.Message,
                Reasons = error.Reasons.Select(x => Map(x)).ToList(),
                Metadata = error.Metadata.ToDictionary(x => x.Key, x => x.Value.ToString())
            };
        }
    }
}