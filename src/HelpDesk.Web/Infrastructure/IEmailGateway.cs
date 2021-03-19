using System.Threading.Tasks;
using HelpDesk.Web.Models;

namespace HelpDesk.Web.Infrastructure
{
    public interface IEmailGateway
    {
        Task SendAsync(Email email);
    }

    public class NoopEmailGateway : IEmailGateway
    {
        public Task SendAsync(Email email)
        {
            return Task.CompletedTask;
        }
    }
}