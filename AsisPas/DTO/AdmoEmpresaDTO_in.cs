using System.ComponentModel.DataAnnotations;

namespace AsisPas.DTO
{
    /// <summary>
    /// para agregar un nueva admo de empresas
    /// </summary>
    public class AdmoEmpresaDTO_in
    {
        /// <summary>
        /// empresa a la que pertenece
        /// </summary>
        [Required]
        public int Empresaid { get; set; }
        /// <summary>
        /// nombre del usuario
        /// </summary>
        [Required(ErrorMessage = "El Nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "no mas de 100 caracteres")]
        public string Nombres { get; set; }
        /// <summary>
        /// apellidos del usuario
        /// </summary>
        [StringLength(100, ErrorMessage = "no mas de 100 caracteres")]
        [Required(ErrorMessage = "El Apellido es obligatorio")]
        public string Apellidos { get; set; }
        /// <summary>
        /// identificacion del usuario
        /// </summary>
        [StringLength(12, ErrorMessage = "no mas de 12 caracteres")]
        [Required(ErrorMessage = "El Rut es obligatorio")]
        public string Rut { get; set; }
        /// <summary>
        /// correo electronico del usuario
        /// </summary>
        [Required(ErrorMessage = "el Correo es necesario")]
        [EmailAddress]
        [StringLength(100, ErrorMessage = "no mas de 100 caracteres")]
        public string Email { get; set; }
        /// <summary>
        /// numero telefonico
        /// </summary>
        [StringLength(15, ErrorMessage = "no mas de 15 caracteres")]
        public string telefono { get; set; }

    }
}
