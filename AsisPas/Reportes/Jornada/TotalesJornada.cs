using System;
using System.Collections.Generic;

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
        /// <summary>
        /// indica los totales diarios
        /// </summary>
        public List<DiaJornada> Dario { get; set; }
        #endregion


        #region ctor
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="inicio"></param>
        /// <param name="fin"></param>
        /// <param name="TiempoAcumuladoEnSegundos"></param>
        /// <param name="dias"></param>
        public TotalesJornada(DateTime inicio, DateTime fin, double TiempoAcumuladoEnSegundos, List<DiaJornada> dias)
        {
            this.inicio = inicio;
            this.fin = fin;
            this.tiempo = new(TiempoAcumuladoEnSegundos);
            this.Dario = new();
            if(dias != null && dias.Count > 0)  
            foreach (var item in dias)
                this.Dario.Add(item);


        }

        #endregion


    }
}
