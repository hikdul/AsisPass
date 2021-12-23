using System;
using System.Collections.Generic;

namespace AsisPas.Reportes.Excesojornada
{
    /// <summary>
    /// lleva los datos semama a semana
    /// </summary>
    public class TotalesExcesoJornadaSemanal
    {
        #region props
        /// <summary>
        /// inicio de la semana
        /// </summary>
        public DateTime inicio{ get; set; }
        /// <summary>
        /// fin de la semana
        /// </summary>
        public DateTime fin { get; set; }
        /// <summary>
        /// tiempo de exceso
        /// </summary>
        public RTime Exceso { get; set; }
        /// <summary>
        /// tiempo de atrazo
        /// </summary>
        public RTime Atrazos { get; set; }
        /// <summary>
        /// calculo de la compensacion
        /// </summary>
        public RTime Compensacion { get; set; }
        /// <summary>
        /// si la compesacion es a favor de la empresa
        /// </summary>
        public bool AfavorEmpresa { get; set; }
        /// <summary>
        /// si este dia es laboral segun horario
        /// </summary>
        public List<DiaExcesoJornada> Diarios { get; set; }

        #endregion


        #region constructor
        /// <summary>
        /// empty
        /// </summary>
        public TotalesExcesoJornadaSemanal()
        {
            this.Exceso = new(0);
            this.Atrazos = new(0);
            this.Diarios = new();
            this.Compensacion = new(0);
            this.AfavorEmpresa = false;
        }
        /// <summary>
        /// ctor valido
        /// </summary>
        /// <param name="inicio"></param>
        /// <param name="fin"></param>
        /// <param name="exceso"></param>
        /// <param name="atrazo"></param>
        /// <param name="diarios"></param>
        public TotalesExcesoJornadaSemanal(DateTime inicio, DateTime fin, double exceso, double atrazo, List<DiaExcesoJornada> diarios)
        {
            this.Diarios = new();
            this.inicio = inicio;
            this.fin = fin;
            this.Exceso = new(exceso);
            this.Atrazos = new(atrazo);
            this.Compensacion = TotalesExcesoJornada.VerCalculoDiferencia(exceso, atrazo);
            foreach (var item in diarios)
                this.Diarios.Add(item);
            this.AfavorEmpresa = atrazo > exceso;
        }
        #endregion

    }
}
