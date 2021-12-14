using AsisPas.Entitys;
using System;

namespace AsisPas.Reportes.Jornada
{
    /// <summary>
    /// almacena las marcas vinculadas
    /// </summary>
    public class horas
    {
        #region props

        /// <summary>
        /// inicio de la jornada
        /// </summary>
        public string InicioJornada { get; set; }
        /// <summary>
        /// fin de la jornada
        /// </summary>
        public string FinJornada { get; set; }
        /// <summary>
        /// inicio del descanzo
        /// </summary>
        public string InicioDescanzo { get; set; }
        /// <summary>
        /// fin del descanzo
        /// </summary>
        public string FinDescazo{ get; set; }

        #endregion


        #region ctor
        /// <summary>
        /// default
        /// </summary>
        public horas()
        {
            

        }

        /// <summary>
        /// por tiempos
        /// </summary>
        /// <param name="inicio"></param>
        /// <param name="fin"></param>
        /// <param name="inicioDes"></param>
        /// <param name="finDes"></param>
        public horas(DateTime inicio, DateTime fin, DateTime inicioDes, DateTime finDes)
        {
            this.InicioDescanzo = inicioDes.ToString("HH:mm:ss");
            this.FinDescazo = finDes.ToString("HH:mm:ss");
            this.InicioJornada = inicio.ToString("HH:mm:ss");
            this.FinJornada = fin.ToString("HH:mm:ss");
        }

        /// <summary>
        /// por medio de un horario
        /// </summary>
        /// <param name="horario"></param>
        public horas(Horario horario)
        {
            this.InicioDescanzo = horario.hbi;
            this.FinDescazo = horario.hbf;
            this.InicioJornada = horario.hi;
            this.FinJornada = horario.hf;

        }

        #endregion


    }
}
