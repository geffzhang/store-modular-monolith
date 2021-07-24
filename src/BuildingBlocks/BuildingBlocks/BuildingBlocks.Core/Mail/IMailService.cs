using System.Threading.Tasks;

namespace BuildingBlocks.Core.Mail
{
    public interface IMailService
    {
        Task SendAsync(MailRequest request);
    }
}