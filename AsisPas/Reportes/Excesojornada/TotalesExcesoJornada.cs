using System;

namespace AsisPas.Reportes.Excesojornada
{
    /// <summary>
    /// indica los totales de tiempos por periodos
    /// </summary>
    public class TotalesExcesoJornada
    {
        #region propiedades
        /// <summary>
        /// inicio
        /// </summary>
        public DateTime inicio { get; set; }
        /// <summary>
        /// fin
        /// </summary>
        public DateTime fin { get; set; }
        /// <summary>
        /// el total de tiempo acumulado
        /// </summary>
        public RTime tiempoExc { get; set; }
        /// <summary>
        /// tiempo de retrazo
        /// </summary>
        public RTime tiempoRet { get; set; }
        /// <summary>
        /// Compensacion
        /// </summary>
        public RTime Compensacion { get; set; }
        #endregion


        #region ctor
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="inicio"></param>
        /// <param name="fin"></param>
        /// <param name="ExcesoSG"></param>
        /// <param name="RetrazoSG"></param>
        public TotalesExcesoJornada(DateTime inicio, DateTime fin, double ExcesoSG, double RetrazoSG)
        {
            this.inicio = inicio;
            this.fin = fin;
            this.tiempoExc = new(ExcesoSG);
            this.tiempoRet = new(RetrazoSG);
        }

        #endregion
    }
}
