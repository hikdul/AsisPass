using AsisPas.Helpers;
using System.ComponentModel.DataAnnotations;

namespace AsisPas.Entitys
{
    /// <summary>
    /// clase para la entidad de horario
    /// </summary>
    public class Horario: Iid,IAct
    {
        #region propiedades
        /// <summary>
        /// id
        /// </summary>
        [Key]
        public int id { get; set; }

        /// <summary>
        /// nombre del horario
        /// </summary>
        [Required(ErrorMessage ="Ingrese un Nombre para el Horraio")]
        [StringLength(25)]
        public string Nombre { get; set; }
        /// <summary>
        /// inicio jornada
        /// </summary>
        [Required(ErrorMessage = "Hora de inincio jornada obligatoria")]
        [StringLength(5)]
        public string hi { get; set; }
        /// <summary>
        /// fin de jordana
        /// </summary>
        [Required(ErrorMessage = "Hora fin de jornada obligatoria")]
        [StringLength(5)]
        public string hf { get; set; }
        /// <summary>
        /// inicio break
        /// </summary>
        [StringLength(5)]
        public string hbi { get; set; }
        /// <summary>
        /// fin break
        /// </summary>
        [StringLength(5)]
        public string hbf { get; set; }
        /// <summary>
        /// indica si la jornada culmina el dia siguiente
        /// </summary>
        public bool diaSiguiente { get; set; } = false;
        /// <summary>
        /// indica si la jornada no tiene descazo
        /// </summary>
        public bool sinDescanzo { get; set; } = true;
        /// <summary>
        /// indica si el horario se encuentra activo
        /// </summary>
        public bool act { get; set; }
        /// <summary>
        /// empresa a la que pertenece el horario
        /// </summary>
        [Required(ErrorMessage ="Empresa a la que pertenece el horario")]
        public int Empresaid { get; set; }
        /// <summary>
        /// prop de navegarion
        /// </summary>
        public Empresa Empresa { get; set; }

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


        #region funciones
        
        /// <summary>
        /// verifica si este dia es laboral o na
        /// </summary>
        /// <param name="dia"></param>
        /// <returns></returns>
        public bool DiaLaboral(int dia)
        {
            switch (dia)
            {
                case 0:
                    return Domingo;
                case 1:
                    return Lunes;
                case 2:
                    return Martes;
                case 3:
                    return Miercoles;
                case 4:
                    return Jueves;
                case 5:
                    return Viernes;
                case 6:
                    return Sabado;
                default:
                    return false;
            }
        }


        #endregion


    }
}
