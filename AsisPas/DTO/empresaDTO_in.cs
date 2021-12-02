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
        [Display(Name = "Nombre de la empresa")]
        public string Nombre { get; set; }
        /// <summary>
        /// rut de la empresa
        /// </summary>
        [Display(Name = "Rut de la empresa")]
        [Required(ErrorMessage = "el Rut es necesario")]
        [StringLength(12)]
        public string Rut { get; set; }
        /// <summary>
        /// rubro en el que participa
        /// </summary>
        [StringLength(30)]
        [Display(Name = "Rubro de la empresa")]
        public string Rubro { get; set; }
        /// <summary>
        /// logo de la empresa
        /// </summary>
        [Display(Name = "Suba imagen o logo")]
        public IFormFile Logo { get; set; }
    }
}
