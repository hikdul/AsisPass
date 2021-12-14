using AsisPas.Reportes.Jornada;
using System;

namespace AsisPas.Reportes.Excesojornada
{
    /// <summary>
    /// para almacenar el dia a dia
    /// </summary>
    public class DiaExcesoJornada
    {
        #region propiedades

        /// <summary>
        /// fecha
        /// </summary>
        public DateTime fecha { get; set; }
        /// <summary>
        /// Horario solicitado
        /// </summary>
        public horas Horario{ get; set; }
        /// <summary>
        /// Marcas solicitada
        /// </summary>
        public horas Marcas { get; set; }
        /// <summary>
        /// tiempo de exceso
        /// </summary>
        public RTime Exceso { get; set; }
        /// <summary>
        /// tiempo de atrazo
        /// </summary>
        public RTime Atrazos { get; set; }
        /// <summary>
        /// si este dia es laboral segun horario
        /// </summary>
        public bool DiaLaboral { get; set; }
        /// <summary>
        /// en caso de ser necesario
        /// </summary>
        public string mensaje { get; set; } = "";

        #endregion


        #region constructor

        /// <summary>
        /// constructor
        /// </summary>
        public DiaExcesoJornada()
        {
            this.Horario = new horas();
            this.Marcas = new horas();
            this.Exceso = new(0);
            this.Atrazos = new(0);
        }


        #endregion

    }
}
