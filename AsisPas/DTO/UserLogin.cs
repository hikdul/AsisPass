namespace AsisPas.DTO
{
    /// <summary>
    /// para que los usuarios se puedan logear
    /// </summary>
    public class UserLogin
    {
        /// <summary>
        /// Email suscrito del ususario
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// psw del usuario
        /// </summary>
        public string Password { get; set; }
    }
}
