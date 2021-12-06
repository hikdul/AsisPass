using AsisPas.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;

namespace AsisPas.Entitys
{
    /// <summary>
    /// registros generados por el empleado
    /// </summary>
    public class Incidencia
    {
        #region propiedades
        /// <summary>
        /// key
        /// </summary>
        [Key]
        public int id { get; set; }
        /// <summary>
        /// si se encuentra activo
        /// </summary>
        public bool act { get; set; } = true;
        /// <summary>
        /// razon por la que se realiza
        /// </summary>
        [Required(ErrorMessage ="debe agregar una explicion")]
        [StringLength(100)]
        public string Razon { get; set; }
        /// <summary>
        /// fecha en que se realiza la solicitud
        /// </summary>
        [Required(ErrorMessage = "indique la fecha")]
        public DateTime fecha { get; set; }
        /// <summary>
        /// hora que expresa
        /// </summary>
        [Required(ErrorMessage ="Indique la hora")]
        public string Hora { get; set; }
        /// <summary>
        /// tipo de marca a la que se le realiza la solicitud
        /// </summary>
        [Required(ErrorMessage ="Indique la marca")]
        public int TipoMarca { get; set; }
        /// <summary>
        /// empleado que hace la solicitud
        /// </summary>
        [Required]
        public int Empleadoid { get; set; }
        /// <summary>
        /// prop de navegacion
        /// </summary>
        public Empleado Empleado { get; set; }
        /// <summary>
        /// define el estado de la incidencia
        /// 0 => solicitada
        /// 1 => Aprobada
        /// 2 => rechazada
        /// </summary>
        [Range(0, 2)]
        public int Estado { get; set; } = 0;
        #endregion

        #region listado por usuario

        /// <summary>
        /// para obtener un listado de incidencias en base al usuario
        /// </summary>
        /// <param name="context"></param>
        /// <param name="User"></param>
        /// <returns></returns>
     
        
        public static async System.Threading.Tasks.Task<List<Incidencia>> ListadoXUsuarioLast15Days(ApplicationDbContext context, ClaimsPrincipal User)
        {
                List<Incidencia> list = new();
            try
            {
                var band = DateTime.Now.AddDays(-15);
                var employ = await Empleado.EmpleadosXUsuario(context, User);
                foreach (var emp in employ)
                {
                    var flag = await context
                        .Incidencias
                        .Include(x => x.Empleado).ThenInclude(x => x.user)
                        .Where(x => x.Empleadoid == emp.id && x.fecha > band )
                        .ToListAsync();
                    foreach (var item in flag)
                        list.Add(item);
                }

                return list;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return list;
            }
        }
        /// <summary>
        /// para obtener las incidencias papa un dia especifico
        /// </summary>
        /// <param name="idEmpleado"></param>
        /// <param name="fecha"></param>
        /// <param name="context"></param>
        /// <param name="User"></param>
        /// <returns></returns>
        public static async System.Threading.Tasks.Task<List<Incidencia>> IncidenciaPorDiaYEmpleado(int idEmpleado, DateTime fecha,ApplicationDbContext context, ClaimsPrincipal User)
        {
            List<Incidencia> list = new();
            try
            {
                return await context.Incidencias
                    .Where(x => x.Empleadoid == idEmpleado && x.fecha.ToString("dd/MM/yyy") == fecha.ToString("dd/MM/yyy"))
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return list;
            }
        }


        #endregion
    }
}
