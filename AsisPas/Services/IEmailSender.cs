using System.Threading.Tasks;

namespace AsisPas.Services
{
    /// <summary>
    /// interface para enviar Email's
    /// </summary>
    public interface IEmailSender
    {
        /// <summary>
        /// metodo de enviar emials
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        Task EmailSender(MailData data);
    }
}
