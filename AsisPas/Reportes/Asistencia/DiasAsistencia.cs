using AsisPas.Entitys;
using System;

namespace AsisPas.Reportes
{
    /// <summary>
    /// para llenar los dias de los reportes de asistencia
    /// </summary>
    public class DiasAsistencia
    {
        #region propiedades
        /// <summary>
        /// fecha en la que se esta estudiando
        /// </summary>
        public string fecha { get; set; }
        /// <summary>
        /// dice si asistio o no
        /// </summary>
        public string Asistio { get; set; }
        /// <summary>
        /// se llena en caso de no haber asistido
        /// </summary>
        public string Observacion { get; set; }
        /// <summary>
        /// permiso
        /// </summary>
        public Permisos permiso { get; set; }
        #endregion

        #region ctor
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="fecha"></param>
        /// <param name="asistio"></param>
        /// <param name="permiso"></param>
        public DiasAsistencia(DateTime fecha, bool asistio, Permisos permiso = null)
        {

            this.fecha = fecha.ToString("dd/MM/yyyy");
            this.Asistio = asistio ? "SI" : "NO";

            if (!asistio && permiso == null)
                this.Observacion = "No Se Registra permiso o justificacion alguna";
            if (!asistio && permiso != null)
                this.Observacion = permiso.Desc;
            this.permiso = permiso;

        }

        #endregion
    }
}
