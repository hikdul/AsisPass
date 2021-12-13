using AsisPas.Entitys;
using System;

namespace AsisPas.Reportes.CambiosTurno
{
    /// <summary>
    /// para mostrar el listado de los cambios de turno
    /// </summary>
    public class modificaciones
    {
        /// <summary>
        /// fecha en la que se realizo la modificacion
        /// </summary>
        public DateTime Modificacion { get; set; }
        /// <summary>
        /// inincio del periodo de la asignacion
        /// </summary>
        public DateTime inicio { get; set; }
        /// <summary>
        /// fin del periodo de asignacion
        /// </summary>
        public DateTime fin { get; set; }
        /// <summary>
        /// Horario que representa el periodo
        /// </summary>
        public Horario Horario{ get; set; }


        public modificaciones()
        {
                this.Horario = new Horario();
        }
    }
}
