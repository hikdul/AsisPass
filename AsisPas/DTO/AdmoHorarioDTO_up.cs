using System;
using System.ComponentModel.DataAnnotations;

namespace AsisPas.DTO
{
    /// <summary>
    /// para actualizar 
    /// </summary>
    public class AdmoHorarioDTO_up
    {
        /// <summary>
        /// id
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// inicio del periodo
        /// </summary>
        [Required(ErrorMessage = "Debe de ingresar una fecha de inicio")]
        public DateTime inicio { get; set; }
        /// <summary>
        /// fin del periodo
        /// </summary>
        [Required(ErrorMessage = "Debe de ingresar una fecha de finalizacion")]
        public DateTime fin { get; set; }

        /// <summary>
        /// empleado al que se le asigna el horario
        /// </summary>
        [Required(ErrorMessage = "Debe de indicar un empleado")]
        public int Empleadoid { get; set; }

        /// <summary>
        /// horario asignado
        /// </summary>
        [Required(ErrorMessage = "Debe de escojer un horario")]
        public int Horarioid { get; set; }

        /// <summary>
        /// razon por la que se realiza el cambio
        /// </summary>
        [Required(ErrorMessage = "Debe definir la razon del combio/asignacion")]
        public string Razon { get; set; }
    }
}
