using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Threading.Tasks;

namespace AsisPas.Services
{
    /// <summary>
    /// servicio que envia los correos electronicos
    /// </summary>
    public class EmailSenderServices : IEmailSender
    {


        #region propiedades y contructor

        private readonly SmtpConfig _smtp;

        /// <summary>
        /// contructor
        /// </summary>
        /// <param name="smtp"></param>
        public EmailSenderServices(IOptions<SmtpConfig> smtp)
        {
            this._smtp = smtp.Value;
        }



        #endregion


        #region funciones

        /// <summary>
        /// funcion para eviar el correo
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task EmailSender(MailData data)
        {
            try
            {

                MimeMessage message = new();
                message.From.Add(new MailboxAddress(_smtp.sendEmail, _smtp.senderEmail));
                message.To.Add(new MailboxAddress(data.Email, data.Email));
                message.Subject = data.Subject;
                message.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = data.Body };

                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(_smtp.server, _smtp.port);
                    await client.AuthenticateAsync(_smtp.senderEmail, _smtp.Password);
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);

                }


            }
            catch (Exception ex)
            {

                Console.WriteLine("Exceptioon Catch!!");
                Console.WriteLine("Exceptioon Message: {0}", ex.Message);

            }

        }

        /// <summary>
        /// other sender
        /// </summary>
        /// <param name="Body"></param>
        /// <param name="fromEmail"></param>
        /// <param name="Subject"></param>
        /// <returns></returns>
        public async Task EmailSender(string Body, string fromEmail, string Subject)
        {
            MailData data = new(Body, fromEmail, Subject);
            await EmailSender(data);
        }

        Task IEmailSender.EmailSender(MailData data)
        {
            throw new System.NotImplementedException();
        }
        #endregion
    }
}
