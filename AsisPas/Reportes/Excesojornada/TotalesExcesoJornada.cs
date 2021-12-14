using System;

namespace AsisPas.Reportes.Excesojornada
{
    /// <summary>
    /// para almacenar la sumatoria de totales de exceso de jornada
    /// </summary>
    public class TotalesExcesoJornada
    {
        #region props
        /// <summary>
        /// inicio del perioda
        /// </summary>
        public DateTime inicio{ get; set; }
        /// <summary>
        /// fin del periodo
        /// </summary>
        public DateTime fin { get; set; }

        /// <summary>
        /// tiempo de exceso
        /// </summary>
        public RTime Exceso { get; set; }
        /// <summary>
        /// tiempo de atrazos
        /// </summary>
        public RTime Atrazos { get; set; }
        /// <summary>
        /// diferencial
        /// </summary>
        public RTime Diferencial { get; set; }
        /// <summary>
        /// true si es a favor de la empresa 
        /// false si es a favor del empleado
        /// </summary>
        public bool AfavorEmpresa { get; set; }

        #endregion


        #region ctor
        /// <summary>
        /// constructar
        /// </summary>
        /// <param name="inicio"></param>
        /// <param name="fin"></param>
        /// <param name="tiempoExcesoEnSegundos"></param>
        /// <param name="tiempoRetrazoEnSegundos"></param>
        public TotalesExcesoJornada(DateTime inicio, DateTime fin,double tiempoExcesoEnSegundos, double tiempoRetrazoEnSegundos)
        {
            this.inicio = inicio;
            this.fin = fin;
            this.Exceso = new(tiempoExcesoEnSegundos);
            this.Atrazos = new(tiempoRetrazoEnSegundos);
            if (tiempoExcesoEnSegundos > tiempoRetrazoEnSegundos)
                AfavorEmpresa = false;
            else
                AfavorEmpresa = true;
            CalcularDiferencia();
        }

        #endregion


        #region calcular diferencia
        /// <summary>
        /// para calcular el diferencial
        /// </summary>
        public void CalcularDiferencia()
        {
            if (this.Exceso == null || this.Atrazos == null)
                this.Diferencial = new(0);

            if (this.Exceso.TiempoEnSegundos > this.Atrazos.TiempoEnSegundos)
                this.Diferencial = new(this.Exceso.TiempoEnSegundos - this.Atrazos.TiempoEnSegundos);
            else
                this.Diferencial = new(this.Atrazos.TiempoEnSegundos - this.Exceso.TiempoEnSegundos);

        }
        /// <summary>
        /// para obtener el calculo de los tiempos
        /// </summary>
        /// <param name="exceso"></param>
        /// <param name="retrazo"></param>
        /// <returns></returns>
        public static RTime VerCalculoDiferencia(double exceso, double retrazo)
        {
            if (exceso> retrazo)
                return new(exceso - retrazo);
            else
                return new(retrazo - exceso);
        }

        #endregion
    }
}
