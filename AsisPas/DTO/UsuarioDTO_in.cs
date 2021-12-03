using System.ComponentModel.DataAnnotations;

namespace AsisPas.DTO
{
    /// <summary>
    /// datos que se solicitaran para ingresar un nuevo usuario
    /// </summary>
    public class UsuarioDTO_in
    {
        /// <summary>
        /// contrasena para ingresar al sistema
        /// </summary>
        [Required]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }
        /// <summary>
        /// nombre del usuario
        /// </summary>
        [Required(ErrorMessage = "El Nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "no mas de 100 caracteres")]
        [Display(Name = "Nombre")]
        public string Nombres { get; set; }
        /// <summary>
        /// apellidos del usuario
        /// </summary>
        [StringLength(100, ErrorMessage = "no mas de 100 caracteres")]
        [Required(ErrorMessage = "El Apellido es obligatorio")]
        [Display(Name = "Apellido")]
        public string Apellidos { get; set; }
        /// <summary>
        /// identificacion del usuario
        /// </summary>
        [StringLength(12, ErrorMessage = "no mas de 12 caracteres")]
        [Required(ErrorMessage = "El Rut es obligatorio")]
        [Display(Name = "RUT")]
        public string Rut { get; set; }
        /// <summary>
        /// correo electronico del usuario
        /// </summary>
        [Required(ErrorMessage = "el Correo es necesario")]
        [EmailAddress]
        [StringLength(100, ErrorMessage = "no mas de 100 caracteres")]
        [Display(Name = "Correo Electronico")]
        public string Email { get; set; }
        /// <summary>
        /// numero telefonico
        /// </summary>
        [StringLength(15, ErrorMessage = "no mas de 15 caracteres")]
        [Display(Name = "Numero Telefonico")]
        public string telefono { get; set; }
    }
}
