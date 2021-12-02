using System.ComponentModel.DataAnnotations;

namespace AsisPas.DTO
{
    /// <summary>
    /// para crear una nueva sede
    /// </summary>
    public class SedeDTO_in
    {
        /// <summary>
        /// nombre de la sede
        /// </summary>
        [Required(ErrorMessage = "Un Nombre es obligatorio")]
        [StringLength(50)]
        [Display(Name = "Ingrese nombre de la sede")]
        public string Nombre { get; set; }

        /// <summary>
        /// direccion de donde se ubica la sede
        /// </summary>
        [Required(ErrorMessage = "La direccion es vital")]
        [StringLength(100)]
        [Display(Name = "Ingrese Direccion de la Sede")]
        public string Direccion { get; set; }
        /// <summary>
        /// empresa a la que pertenece la sede
        /// </summary>
        [Required(ErrorMessage = "La Sede necesita su empresa de destino")]
        [Display(Name = "Seleccione Empresa")]
        public int Empresaid { get; set; }


    }
}
