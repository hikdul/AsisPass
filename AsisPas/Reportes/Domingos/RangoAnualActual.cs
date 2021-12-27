using System.Collections.Generic;

namespace AsisPas.Reportes.Domingos
{
    /// <summary>
    /// estudio por ano
    /// </summary>
    public class RangoAnualActual: RangoTotalesDomingo
    {

        #region props
        /// <summary>
        /// es estudio mes a mes
        /// </summary>
        public List<MensualDomingos> Mensual { get; set; }
        #endregion


        #region ctor
        /// <summary>
        /// empty ctor
        /// </summary>
        public RangoAnualActual()
        {
            this.Mensual = new();
        }
        /// <summary>
        /// para llenar los datos una ves se tengan por ano
        /// </summary>
        /// <param name="totales"></param>
        /// <param name="mensuals"></param>
        public RangoAnualActual(RangoTotalesDomingo totales, List<MensualDomingos> mensuals)
        {
            this.Mensual = new();
            this.inicio = totales.inicio;
            this.fin = totales.fin;
            this.totalDomingosenPeriodo = totales.totalDomingosenPeriodo;
            this.totalFeriadosenPeriodo = totales.totalFeriadosenPeriodo;
            this.totalDomingos = totales.totalDomingos;
            this.totalFeriados = totales.totalFeriados;
            foreach (var item in mensuals)
                this.Mensual.Add(item);
        }

        #endregion

    }
}
