using System.Threading.Tasks;
using Mercury.Abstraction.Models;

namespace Mercury.Abstraction.Interfaces
{
    public interface IEmailer
    {
        Task SendAsync(EmailMessage message);
    }
}