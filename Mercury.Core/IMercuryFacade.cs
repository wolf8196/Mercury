using System.Threading.Tasks;
using Mercury.Abstraction.Models;

namespace Mercury.Core
{
    public interface IMercuryFacade
    {
        Task SendAsync(EmailRequest request);
    }
}