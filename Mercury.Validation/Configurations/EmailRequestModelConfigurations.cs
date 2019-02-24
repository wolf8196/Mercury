using System.Linq;
using FluentObjectValidator.ModelConfiguration;
using FluentObjectValidator.RuleExtensions;
using Mercury.Abstraction.Models;

namespace Mercury.Validation.Configurations
{
    public class EmailRequestModelConfigurations : ValidationConfiguration<EmailRequest>
    {
        public EmailRequestModelConfigurations()
        {
            Property(x => x.TemplateKey)
                .IsRequired();

            Property(x => x.Tos)
                .IsRequired()
                .HasRule(x => x.Any());

            Property(x => x.Payload)
                .IsRequired();
        }
    }
}