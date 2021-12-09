using System;

namespace AsisPas.Reportes.Domingos
{
    /// <summary>
    /// para obtener los totales por periodo
    /// </summary>
    public class RangoTotalesDomingo
    {
        /// <summary>
        /// inicio
        /// </summary>
        public DateTime inicio { get; set; }
        /// <summary>
        /// fin
        /// </summary>
        public DateTime fin { get; set; }
        /// <summary>
        /// la cantidad de dias demingos laborados
        /// </summary>
        public int totalDomingos { get; set; } = 0;
        /// <summary>
        /// el total de feriados laborados
        /// </summary>
        public int totalFeriados { get; set; } = 0;
        /// <summary>
        /// total domingos en periodo
        /// </summary>
        public int totalDomingosenPeriodo { get; set; } = 0;
        /// <summary>
        /// total feriados en periodo
        /// </summary>
        public int totalFeriadosenPeriodo { get; set; } = 0;
    }
}
