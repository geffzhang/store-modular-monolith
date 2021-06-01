using System.Threading.Tasks;

namespace Common.Mail
{
    public interface IMailService
    {
        Task SendAsync(MailRequest request);
    }
}