using System;

namespace AsisPas.Reportes.Jornada
{
    /// <summary>
    /// totales por jornada
    /// </summary>
    public class TotalesJornada
    {
        #region propiedades
        /// <summary>
        /// inicio
        /// </summary>
        public DateTime inicio{ get; set; }
        /// <summary>
        /// fin
        /// </summary>
        public DateTime fin { get; set; }
        /// <summary>
        /// el total de tiempo acumulado
        /// </summary>
        public RTime tiempo { get; set; }
        #endregion


        #region ctor
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="inicio"></param>
        /// <param name="fin"></param>
        /// <param name="TiempoAcumuladoEnSegundos"></param>
        public TotalesJornada(DateTime inicio, DateTime fin, double TiempoAcumuladoEnSegundos)
        {
            this.inicio = inicio;
            this.fin = fin;
            this.tiempo = new(TiempoAcumuladoEnSegundos);
        }

        #endregion


    }
}
