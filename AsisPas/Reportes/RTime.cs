using System;

namespace AsisPas.Reportes
{
    /// <summary>
    /// para manipular las tiempos para las vistas que lo utilicen
    /// </summary>
    public class RTime
    {
        #region props
        /// <summary>
        /// cantidad de horas
        /// </summary>
        public int Hora { get; set; }
        /// <summary>
        /// cantidad de minutos
        /// </summary>
        public int min { get; set; }
        /// <summary>
        /// cantidad de segundos
        /// </summary>
        public int seg { get; set; }
        /// <summary>
        /// el tiempo total en segundos
        /// </summary>
        public double TiempoEnSegundos { get; set; }
        #endregion


        #region ctor

        /// <summary>
        /// constructor, pasandole el total de segundos
        /// </summary>
        /// <param name="TiempoEnSegundos"></param>
        public RTime(double TiempoEnSegundos)
        {
            if (TiempoEnSegundos < 0)
                TiempoEnSegundos = -TiempoEnSegundos;

            this.TiempoEnSegundos = TiempoEnSegundos;
            this.Hora = (int)(TiempoEnSegundos / 3600);
            this.min = (int)(TiempoEnSegundos - (this.Hora * 3600)) / 60;
            this.seg = (int)( TiempoEnSegundos - ((this.Hora * 3600) + (this.min * 60)));
        }

        #endregion


        #region toString

        /// <summary>
        /// para obtener el valor de modo visual
        /// </summary>
        /// <returns></returns>
        public string toStr()
        {
            return $"{this.Hora}:{this.min}:{this.seg}";
        }

        #endregion

        #region Calcular El Total De Tiempo Laborado
        /// <summary>
        /// para obtener un valor en double
        /// </summary>
        /// <param name="inicio"></param>
        /// <param name="fin"></param>
        /// <param name="Dinicio"></param>
        /// <param name="Dfin"></param>
        /// <param name="mismodia"></param>
        /// <returns></returns>
        public static double ObtenerTotalSg(DateTime inicio, DateTime fin, DateTime Dinicio, DateTime Dfin, bool mismodia = true)
        {
            

            if (mismodia)
            {
                var totalJornada = fin.Subtract(inicio);
                var totalDescanso = Dfin.Subtract(Dinicio);
                return (totalJornada - totalDescanso).TotalSeconds;
            }
            return 0;
        }


        /// <summary>
        /// para obtener resultados en base a un complemento entregado
        /// </summary>
        /// <param name="inicio"></param>
        /// <param name="fin"></param>
        /// <param name="Dinicio"></param>
        /// <param name="Dfin"></param>
        /// <param name="mismodia"></param>
        /// <returns></returns>
        public static RTime CalcularTiempos(DateTime inicio, DateTime fin, DateTime Dinicio, DateTime Dfin, bool mismodia = true)
        {
            if (mismodia)
            {
                var totalJornada = fin.Subtract(inicio);
                var totalDescanso = Dfin.Subtract(Dinicio);
                return new((totalJornada - totalDescanso).TotalSeconds);
            }
            return new(0);
        }
        /// <summary>
        /// para obtener el valor en string
        /// </summary>
        /// <param name="inicio"></param>
        /// <param name="fin"></param>
        /// <param name="Dinicio"></param>
        /// <param name="Dfin"></param>
        /// <param name="mismodia"></param>
        /// <returns></returns>
        public static string CalcularTiemposToStr(DateTime inicio, DateTime fin, DateTime Dinicio, DateTime Dfin, bool mismodia = true)
        {
                return CalcularTiempos(inicio,fin,Dinicio,Dfin,mismodia).toStr();
        }

        #endregion


    }
}
