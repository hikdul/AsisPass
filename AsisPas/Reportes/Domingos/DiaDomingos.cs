using System;

namespace AsisPas.Reportes.Domingos
{
    /// <summary>
    /// para obtener la informacion base de un dia
    /// </summary>
    public class DiaDomingos
    {
        /// <summary>
        /// fecha de estudio
        /// </summary>
        public DateTime fecha { get; set; }
        /// <summary>
        /// SI asistio o NO
        /// </summary>
        public string Asistio { get; set; }
        /// <summary>
        /// algun mensaje o descripcion
        /// </summary>
        public string Observacion { get; set; }
    }
}
