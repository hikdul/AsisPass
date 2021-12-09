using System.Collections.Generic;

namespace AsisPas.Reportes.Domingos
{
    /// <summary>
    /// datos por mes
    /// </summary>
    public class MensualDomingos : RangoTotalesDomingo
    {
        /// <summary>
        /// listado de dias
        /// </summary>
        public List<DiaDomingos> Diario { get; set; }
    }
}
