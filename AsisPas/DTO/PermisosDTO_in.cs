using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace AsisPas.DTO
{
    /// <summary>
    /// para agregar un permiso al sistema
    /// </summary>
    public class PermisosDTO_in
    {
        /// <summary>
        /// descrigcion del permiso
        /// </summary>
        [Required(ErrorMessage = "explique brevemente el permiso otorgado")]
        [StringLength(100, ErrorMessage = "breve son maximo 100 caracteres")]
        [MinLength(5, ErrorMessage = "Al menos agrege 5 caracteres como explicacion")]
        public string Desc { get; set; }
        /// <summary>
        /// alguna imagen que desee acompañar
        /// </summary>
        public IFormFile PruebaFotografica { get; set; }
        /// <summary>
        /// inicio del periodo del permiso
        /// </summary>
        [Required(ErrorMessage = "indique la fecha de inicio")]
        public DateTime inicio { get; set; }
        /// <summary>
        /// fin del periodo del permiso
        /// </summary>
        [Required(ErrorMessage = "Indique la fecha en que acaba")]
        public DateTime fin { get; set; }
        /// <summary>
        /// Empleado al que se le otorga el permiso
        /// </summary>
        [Required(ErrorMessage = "Un Epleado es necesario")]
        public int Empleadoid { get; set; }
    }
}
