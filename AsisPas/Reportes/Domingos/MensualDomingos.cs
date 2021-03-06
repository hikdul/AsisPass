using System.Collections.Generic;

namespace AsisPas.Reportes.Domingos
{
    /// <summary>
    /// datos por mes
    /// </summary>
    public class MensualDomingos : RangoTotalesDomingo
    {
        #region props
        /// <summary>
        /// listado de dias
        /// </summary>
        public List<DiaDomingos> Diario { get; set; }
        #endregion

        #region ctor
        /// <summary>
        /// empty
        /// </summary>
        public MensualDomingos()
        {
            this.Diario = new();
        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="totales"></param>
        /// <param name="days"></param>
        public MensualDomingos(RangoTotalesDomingo totales ,List<DiaDomingos> days)
        {
            this.Diario=new();
            this.inicio = totales.inicio;
            this.fin = totales.fin;
            this.totalDomingosenPeriodo = totales.totalDomingosenPeriodo;
            this.totalFeriadosenPeriodo = totales.totalFeriadosenPeriodo;
            this.totalDomingos = totales.totalDomingos;
            this.totalFeriados = totales.totalFeriados;
            foreach (var item in days)
                this.Diario.Add(item);
        }


        #endregion
    }
}
