using AsisPas.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace AsisPas.Reportes
{
    /// <summary>
    /// para generar toda la data necesaria en la busqueda de reportes.
    /// </summary>
    public class ElementosBusqueda
    {
        #region props
        /// <summary>
        /// id del empleado
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// nombres del empleado
        /// </summary>
        public string nombres { get; set; }
        /// <summary>
        /// apellidos del empleado
        /// </summary>
        public string Apellidos { get; set; }
        /// <summary>
        /// rut del empleado
        /// </summary>
        public string Rut { get; set; }
        /// <summary>
        /// Faena actual
        /// </summary>
        public string Faena { get; set; }
        /// <summary>
        /// faena id
        /// </summary>
        public string Faenaid { get; set; }
        /// <summary>
        /// turno o nombre de horario
        /// </summary>
        public string Turno { get; set; }
        /// <summary>
        /// horario id
        /// </summary>
        public string Turnoid { get; set; }
        /// <summary>
        /// primer registro obtenido en la BD
        /// </summary>
        public DateTime PrimerRegistro{ get; set; }
        #endregion

       


        #region llenar data
        /// <summary>
        /// llena los registros
        /// </summary>
        /// <param name="idEmpleado"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task Up(int idEmpleado, ApplicationDbContext context)
        {

            var empleado = await context.Empleados
                .Include(x => x.user)
                .FirstOrDefaultAsync(x => x.id == idEmpleado);

            this.id = empleado.id;
            this.nombres = empleado.user.Nombres;
            this.Apellidos = empleado.user.Apellidos;
            this.Rut = empleado.user.Rut;

            var first = await context.Marcaciones.FirstOrDefaultAsync(x => x.Empleadoid == idEmpleado);
            var lastc = await context.Marcaciones
                .Include(x => x.Horario)
                .Include(x => x.Sede)
                .Where(x => x.Empleadoid == idEmpleado).ToListAsync();
                
            var last = lastc == null || lastc.Count > 0 ? lastc[lastc.Count -1] : null;

            if(first == null || last == null)
            {
                this.Faena = this.Turno = "S/R";
                this.Faenaid = this.Turnoid = "0";
                this.PrimerRegistro = DateTime.Now;
            }
            else
            {
                this.Faena = last.Sede.Nombre;
                this.Turno = last.Horario.Nombre;
                this.Faenaid = last.Sedeid.ToString();
                this.Turnoid = last.Horario.id.ToString();

                this.PrimerRegistro = first.marca;
            }


        }


        #endregion
    }
}
