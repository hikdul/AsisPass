using AsisPas.Helpers;
using System;
using System.ComponentModel.DataAnnotations;

namespace AsisPas.Entitys
{
    /// <summary>
    /// cambios generados y validados por el administrador de empresa
    /// </summary>
    public class Cambios: Iid,IAct
    {
        #region propiedades
        /// <summary>
        /// id
        /// </summary>
        [Key]
        public int id { get; set; }
        /// <summary>
        /// act
        /// </summary>
        public bool act { get; set; } = true;
        /// <summary>
        /// si ya se estudio el caso o no
        /// </summary>
        [Required(ErrorMessage ="Debe De dar una razon")]
        public string Razon { get; set; }
        /// <summary>
        /// fecha en la que aplica el cambio
        /// </summary>
        [Required(ErrorMessage ="Fecha para la que aplica el cambio es necesaria")]
        public DateTime fecha { get; set; }
        /// <summary>
        /// fecha en el que fue generado
        /// </summary>
        public DateTime fechaGenerado { get; set; } = DateTime.Now;
        /// <summary>
        /// hora de ingreso jornada
        /// </summary>
        [Required(ErrorMessage ="La hora es necesaria")]
        public string Hora { get; set; }
        /// <summary>
        /// tipo marca
        /// </summary>
        public int tipoMarca { get; set; } = 0; 

        /// <summary>
        /// empleado al que se le realiza el cambio
        /// </summary>
        [Required]
        public int Empleadoid { get; set; }
        /// <summary>
        /// prop nav
        /// </summary>
        public Empleado Empleado { get; set; }

        /// <summary>
        /// para insertar una prueba fotografica.
        /// </summary>
        public string  PruebaFotografica{ get; set; }
        #endregion
    }
}
