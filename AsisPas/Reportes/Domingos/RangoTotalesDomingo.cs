using System;

namespace AsisPas.Reportes.Domingos
{
    /// <summary>
    /// para obtener los totales por periodo
    /// </summary>
    public class RangoTotalesDomingo
    {
        #region props
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
        #endregion


        #region ctor
        /// <summary>
        /// empty
        /// </summary>
        public RangoTotalesDomingo()
        {

        }
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="inicio"></param>
        /// <param name="fin"></param>
        /// <param name="Dtrabajados"></param>
        /// <param name="Ftrabajados"></param>
        /// <param name="Dperiodo"></param>
        /// <param name="Fperiodo"></param>
        public RangoTotalesDomingo(DateTime inicio, DateTime fin, int Dtrabajados, int Ftrabajados, int Dperiodo, int Fperiodo)
        {

            this.inicio = inicio;
            this.fin = fin; 
            this.totalFeriadosenPeriodo = Fperiodo;
            this.totalDomingosenPeriodo = Dperiodo;
            this.totalDomingos = Dtrabajados;
            this.totalFeriados = Ftrabajados;

        }


        #endregion
    }
}
