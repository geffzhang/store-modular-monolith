using System.Threading.Tasks;

namespace Common.Core.Mail
{
    public interface IMailService
    {
        Task SendAsync(MailRequest request);
    }
}