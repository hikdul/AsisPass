using System;
using System.ComponentModel.DataAnnotations;

namespace AsisPas.DTO
{
    /// <summary>
    /// para agregar un feriado
    /// </summary>
    public class FeriadoDTO_in
    {
        /// <summary>
        /// descripcion o contesto histarico
        /// </summary>
        [Required(ErrorMessage = "agrege una desc del dia")]
        [StringLength(50)]
        public string desc { get; set; }
        /// <summary>
        /// en el caso de feriados que se declaran como nuevo dia
        /// </summary>
        public bool UnaSolaVes { get; set; } = false;
        /// <summary>
        /// fecha en la que se aplica el feriado
        /// </summary>
        [Required(ErrorMessage = "La Fecha es importante")]
        public DateTime fecha { get; set; }
    }
}
