using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace AsisPas.DTO
{
    /// <summary>
    /// para agregar un nuevo cambio
    /// </summary>
    public class CambiosDTO_in
    {
        /// <summary>
        /// si ya se estudio el caso o no
        /// </summary>
        [Required(ErrorMessage = "Debe De dar una razon")]
        public string Razon { get; set; }
        /// <summary>
        /// fecha en la que aplica el cambio
        /// </summary>
        [Required(ErrorMessage = "Fecha para la que aplica el cambio es necesaria")]
        public DateTime fecha { get; set; }
        /// <summary>
        /// hora de ingreso jornada
        /// </summary>
        [Required(ErrorMessage = "La hora es necesaria")]
        public string Hora { get; set; }
        /// <summary>
        /// tipo marca
        /// </summary>
        [Required(ErrorMessage = "Seleccione el tipo de marca")]
        [Range(0,4)]
        public int tipoMarca { get; set; } = 0;

        /// <summary>
        /// empleado al que se le realiza el cambio
        /// </summary>
        [Required]
        public int Empleadoid { get; set; }

        /// <summary>
        /// para insertar una prueba fotografica.
        /// </summary>
        public IFormFile PruebaFotografica { get; set; }
    }
}
