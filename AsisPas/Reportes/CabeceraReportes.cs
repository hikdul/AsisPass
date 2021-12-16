using AsisPas.Data;
using AsisPas.Entitys;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace AsisPas.Reportes
{
    /// <summary>
    /// contiene los datos que tienen todos los reportes
    /// </summary>
    public class CabeceraReportes
    {
        #region prop
        /// <summary>
        /// el id del empleado
        /// </summary>
        public int idEmpleado { get; set; }
        /// <summary>
        /// fecha en que se genero
        /// </summary>
        public DateTime FechaGenerado { get; set; } = DateTime.Now;
        /// <summary>
        /// nombre del empleado
        /// </summary>
        public string NombreEmpleado { get; set; }
        /// <summary>
        /// Rut del empleado
        /// </summary>
        public string RutEmpleado { get; set; }
        /// <summary>
        /// Nombre Empresa
        /// </summary>
        public string NombreEmpresa { get; set; }
        /// <summary>
        /// rut de la empresa
        /// </summary>
        public string RutEmpresa { get; set; }
        /// <summary>
        /// inicio del perioda de busqueda del empleado
        /// </summary>
        public DateTime inicio { get; set; }
        /// <summary>
        /// fin del perioda de busquedo del empleado
        /// </summary>
        public DateTime fin { get; set; }
        /// <summary>
        /// nombre de la sede dondo esta actualmente el empleado
        /// </summary>

        public string NombreSede { get; set; }
        /// <summary>
        /// direccion de la sede actual
        /// </summary>
        public string DireccionDelaSede { get; set; }
        /// <summary>
        /// Nombre del horario
        /// </summary>
        public string NombreHorario { get; set; }
        /// <summary>
        /// mensaje es caso de querer avisar algo
        /// </summary>
        public string Mensaje { get; set; } = "";
        #endregion

        #region llenar datos
        /// <summary>
        /// para llenar los datos de la cabecera
        /// </summary>
        /// <param name="idEmpleado"></param>
        /// <param name="inicio"></param>
        /// <param name="fin"></param>
        /// <param name="first"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task UpHead(int idEmpleado, DateTime inicio, DateTime fin,Marca first, ApplicationDbContext context)
        {
            try
            {
                // ### LLENO LA CABECERA
                var marca = await context.Marcaciones
                    .Where(x => x.Empleadoid == idEmpleado).OrderByDescending(x => x.marca)
                    .FirstOrDefaultAsync();
               
                if(first != null && first.marca > inicio)
                {
                  string ff = first.marca.ToString("dd/MM/yyyy");
                  this.Mensaje = $"Todo estudio Anterior a la fecha {ff} fue ignorado, La razon es por que no hay registros anteriores a la fecha mencionada y es posible que el empleado no estuviera contratado aun.";
                  inicio = first.marca;   
                }


                int sede = marca == null ? 0 : marca.Sedeid;
                // lleno mis fechas
                this.FechaGenerado = DateTime.Now;
                this.inicio = inicio;
                this.fin = fin;
                // saco los datos de un empleado
                var user = await context.Empleados
                    .Include(x => x.user)
                    .Include(x => x.Empresa)
                    .FirstOrDefaultAsync(x => x.id == idEmpleado);
                if (user == null)
                    return;
                this.idEmpleado = user.id;
                this.NombreEmpleado = $"{user.user.Nombres} {user.user.Apellidos}";
                this.RutEmpleado = user.user.Rut;
                this.NombreEmpresa = user.Empresa.Nombre;
                this.RutEmpresa = user.Empresa.Rut;

                //llena los datos de Horario y sede
                var fah = await context.AdmoHorarios.Include(x => x.Horario).Where(x => x.Empleadoid == idEmpleado).ToListAsync();
                this.NombreHorario = fah == null || fah.Count < 1
                    ? "No Hay Horarios Registrado"
                    : fah[fah.Count - 1].Horario.Nombre;
                var fsede = await context.Sedes.FirstOrDefaultAsync(x => x.id == sede);
                this.DireccionDelaSede = fsede == null || fsede.id < 1 ? "No Hay Sede Registrada" : fsede.Direccion;
                this.NombreSede = fsede == null || fsede.id < 1 ? "No Hay Sede Registrada" : fsede.Nombre;
                //if (fah != null && fah.Count > 0 && fah[0].inicio < inicio)
                //    this.Mensaje = $"Para la fecha {inicio.ToString("dd/MM/yyyy")} No hay Registros... posiblemente el empleado aun no estaba contratado";

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        #endregion

    }
}
