using System.ComponentModel.DataAnnotations;

namespace AsisPas.DTO
{
    /// <summary>
    /// para agregar un nueva horario
    /// </summary>
    public class HorarioDTO_in
    {
        #region propiedades


        /// <summary>
        /// nombre del horario
        /// </summary>
        [Required(ErrorMessage = "Ingrese un Nombre para el Horraio")]
        [StringLength(25)]
        [Display(Name ="Nombre")]
        public string Nombre { get; set; }
        /// <summary>
        /// inicio jornada
        /// </summary>
        [Required(ErrorMessage = "Hora de inincio jornada obligatoria")]
        [StringLength(5)]
        [Display(Name = "Hora Inicio Jornada")]
        public string hi { get; set; }
        /// <summary>
        /// fin de jordana
        /// </summary>
        [Required(ErrorMessage = "Hora fin de jornada obligatoria")]
        [StringLength(5)]
        [Display(Name = "Hora Fin jornada")]
        public string hf { get; set; }
        /// <summary>
        /// inicio break
        /// </summary>
        [StringLength(5)]
        [Display(Name = "Hora Inicio Descanzo")]
        public string hbi { get; set; }
        /// <summary>
        /// fin break
        /// </summary>
        [StringLength(5)]
        [Display(Name = "Hora fin Descanzo")]
        public string hbf { get; set; }
        /// <summary>
        /// indica si la jornada culmina el dia siguiente
        /// </summary>
        [Display(Name = "fin de jornada es al siguiente dia")]
        public bool diaSiguiente { get; set; } = false;
        /// <summary>
        /// indica si la jornada no tiene descazo
        /// </summary>
        [Display(Name = "Esta jornada no lleva descanzo")]
        public bool sinDescanzo { get; set; } = true;
        /// <summary>
        /// empresa a la que pertenece el horario
        /// </summary>
        [Required(ErrorMessage = "Empresa a la que pertenece el horario")]
        [Display(Name = "Seleccione Empresa")]
        public int Empresaid { get; set; }
        /// <summary>
        /// indica si aplica para el dia
        /// </summary>
        
        public bool Domingo { get; set; }

        /// <summary>
        /// indica si aplica para el dia
        /// </summary>
        public bool Lunes { get; set; }

        /// <summary>
        /// indica si aplica para el dia
        /// </summary>
        public bool Martes { get; set; }

        /// <summary>
        /// indica si aplica para el dia
        /// </summary>
        public bool Miercoles { get; set; }

        /// <summary>
        /// indica si aplica para el dia
        /// </summary>
        public bool Jueves { get; set; }

        /// <summary>
        /// indica si aplica para el dia
        /// </summary>
        public bool Viernes { get; set; }

        /// <summary>
        /// indica si aplica para el dia
        /// </summary>
        public bool Sabado { get; set; }


        #endregion

    }
}
