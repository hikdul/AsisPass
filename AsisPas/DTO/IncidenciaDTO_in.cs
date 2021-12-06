using System;
using System.ComponentModel.DataAnnotations;

namespace AsisPas.DTO
{
    /// <summary>
    /// datos necesarios para crear una incidencia
    /// </summary>
    public class IncidenciaDTO_in
    {
        /// <summary>
        /// razon por la que se realiza
        /// </summary>
        [Required(ErrorMessage = "debe agregar una explicion")]
        [StringLength(100)]
        public string Razon { get; set; }
        /// <summary>
        /// fecha en que se realiza la solicitud
        /// </summary>
        [Required(ErrorMessage = "indique la fecha")]
        public DateTime fecha { get; set; }
        /// <summary>
        /// hora que expresa
        /// </summary>
        [Required(ErrorMessage = "Indique la hora")]
        public string Hora { get; set; }
        /// <summary>
        /// tipo de marca a la que se le realiza la solicitud
        /// </summary>
        [Required(ErrorMessage = "Indique la marca")]
        public int TipoMarca { get; set; }



    }
}
