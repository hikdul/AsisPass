using System;

namespace AsisPas.Reportes.Jornada
{
    /// <summary>
    /// para almacenar los datos de un dia en jornada
    /// </summary>
    public class DiaJornada
    {
        /// <summary>
        /// fecha de estudio
        /// </summary>
        public DateTime fecha { get; set; }
        /// <summary>
        /// horario del dia
        /// </summary>
        public horas horario { get; set; }
        /// <summary>
        /// marcas del dia
        /// </summary>
        public horas marcas { get; set; }
        /// <summary>
        /// tiempo laborado
        /// </summary>
        public string tiempoLaborado { get; set; }
    }
}
