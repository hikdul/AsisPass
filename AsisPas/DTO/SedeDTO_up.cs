using System.ComponentModel.DataAnnotations;

namespace AsisPas.DTO
{
    /// <summary>
    /// para actualizar los datos de una sede
    /// </summary>
    public class SedeDTO_up
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
        public string Nombre { get; set; }

        /// <summary>
        /// direccion de donde se ubica la sede
        /// </summary>
        [Required(ErrorMessage = "La direccion es vital")]
        [StringLength(100)]
        public string Direccion { get; set; }
        /// <summary>
        /// empresa a la que pertenece la sede
        /// </summary>
        [Required(ErrorMessage = "La Sede necesita su empresa de destino")]
        public int Empresaid { get; set; }

    }
}
