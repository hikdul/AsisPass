using AsisPas.Reportes.Jornada;
using System;

namespace AsisPas.Reportes.Excesojornada
{
    /// <summary>
    /// para almacenar el dia a dia
    /// </summary>
    public class DiaExcesoJornada
    {
        /// <summary>
        /// fecha
        /// </summary>
        public DateTime fecha { get; set; }
        /// <summary>
        /// Horario solicitado
        /// </summary>
        public horas Horario{ get; set; }
        /// <summary>
        /// Marcas solicitada
        /// </summary>
        public horas Marcas { get; set; }
        /// <summary>
        /// tiempo de exceso
        /// </summary>
        public horas calculos { get; set; }

    }
}
