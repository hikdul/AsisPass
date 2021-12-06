using System;
using System.ComponentModel.DataAnnotations;

namespace AsisPas.DTO
{
    /// <summary>
    /// para ingresar una nueva marca por medio de la api
    /// </summary>
    public class MarcaDTO_in
    {
        #region propiedades
        /// <summary>
        /// rut del empleado
        /// </summary>
        [Required(ErrorMessage ="el Rut es obligatorio")]
        public string Rut { get; set; }
        /// <summary>
        /// tiempo en fecha y hora 
        /// </summary>
        [Required(ErrorMessage = "La fecha y hora son necesarias")]
        public DateTime marca { get; set; }
       /// <summary>
       /// codigo de la puenta
       /// </summary>
        [Required(ErrorMessage = "El Codigo de la puerta es obligatorio")]
        public string GateCode { get; set; }
        /// <summary>
        /// tipo de ingreso
        /// 1 => Inicio de jornada
        /// 2 => Inicio de descanzo
        /// 3 => fin del descanzo
        /// 4 => fin de la jornada
        /// 5 => undefined
        /// </summary>
        [Required(ErrorMessage = "el tipo de marca es necesario")]
        [Range(0,5,ErrorMessage ="el rango es de 0 a 5")]
        public int TipoIngreso { get; set; }


        #endregion
    }
}
