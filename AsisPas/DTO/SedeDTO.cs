using System.ComponentModel.DataAnnotations;

namespace AsisPas.DTO
{
    /// <summary>
    /// para enviar los datos de la sede
    /// </summary>
    public class SedeDTO
    {
        /// <summary>
        /// id
        /// </summary>
        [Key]
        public int id { get; set; }
        /// <summary>
        /// nombre de la sede
        /// </summary>
        [Required(ErrorMessage = "Un Nombre es obligatorio")]
        [StringLength(50)]
        [Display(Name = "Sede")]
        public string Nombre { get; set; }

        /// <summary>
        /// direccion de donde se ubica la sede
        /// </summary>
        [Required(ErrorMessage = "La direccion es vital")]
        [StringLength(100)]
        [Display(Name = "Direccion")]
        public string Direccion { get; set; }
        /// <summary>
        /// empresa a la que pertenece la sede
        /// </summary>
        [Required(ErrorMessage = "La Sede necesita su empresa de destino")]
        [Display(Name = "Empresa")]
        public string NombreEmpresa{ get; set; }
       

    }
}
