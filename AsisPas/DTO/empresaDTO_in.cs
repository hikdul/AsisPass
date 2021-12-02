using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace AsisPas.DTO
{
    /// <summary>
    /// para agregar datos de lo empresa
    /// </summary>
    public class empresaDTO_in
    {

        /// <summary>
        /// nombre de la empresa
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Nombre { get; set; }
        /// <summary>
        /// rut de la empresa
        /// </summary>
        [Required(ErrorMessage = "el Rut es necesario")]
        [StringLength(12)]
        public string Rut { get; set; }
        /// <summary>
        /// rubro en el que participa
        /// </summary>
        [StringLength(30)]
        public string Rubro { get; set; }
        /// <summary>
        /// logo de la empresa
        /// </summary>
        public IFormFile Logo { get; set; }
    }
}
