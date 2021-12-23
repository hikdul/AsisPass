namespace AsisPas.Services
{
    public class MailData
    {

        #region propiedades
        /// <summary>
        /// enmail de destino
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// subject 
        /// </summary>
        public string Subject { get; set; }
        /// <summary>
        /// body o cuerppo
        /// </summary>
        public string Body { get; set; }
        #endregion

        #region contructor
        /// <summary>
        /// default
        /// </summary>
        public MailData()
        {

        }
        /// <summary>
        /// complete
        /// </summary>
        /// <param name="Body"></param>
        /// <param name="email"></param>
        /// <param name="Subject"></param>
        public MailData(string Body, string email, string Subject)
        {
            this.Body = Body;
            this.Email = email;
            this.Subject = Subject;
        }
        #endregion


    }
}
