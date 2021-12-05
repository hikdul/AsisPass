using AsisPas.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AsisPas.Entitys
{
    /// <summary>
    /// para almacenar los horarios asignados a empleados
    /// </summary>
    public class AdmoHorario
    {
        #region prop
        /// <summary>
        /// id
        /// </summary>
        [Key]
        public int id { get; set; }

        /// <summary>
        /// fecha en que se asigno el horario
        /// </summary>
        public DateTime fechaAsignacion { get; set; } = DateTime.Now;
        /// <summary>
        /// inicio del periodo
        /// </summary>
        [Required(ErrorMessage ="Debe de ingresar una fecha de inicio")]
        public DateTime inicio{ get; set; }
        /// <summary>
        /// fin del periodo
        /// </summary>
        [Required(ErrorMessage = "Debe de ingresar una fecha de finalizacion")]
        public DateTime fin { get; set; }
        /// <summary>
        /// prop nav
        /// </summary>
        public Empleado Empleado { get; set; }
        /// <summary>
        /// empleado al que se le asigna el horario
        /// </summary>
        [Required(ErrorMessage = "Debe de indicar un empleado")]
        public int Empleadoid { get; set; }
        /// <summary>
        /// porp nav
        /// </summary>

        public Horario Horario { get; set; }
        /// <summary>
        /// horario asignado
        /// </summary>
        [Required(ErrorMessage = "Debe de escojer un horario")]
        public int Horarioid { get; set; }
        /// <summary>
        /// prop nav
        /// </summary>
        public AdmoEmpresas Admo { get; set; }
        /// <summary>
        /// administrador que asigno el horario
        /// </summary>
        [Required(ErrorMessage = "Debe identificarse")]
        public int Admoid { get; set; }
        /// <summary>
        /// razon por la que se realiza el cambio
        /// </summary>
        [Required(ErrorMessage = "Debe definir la razon del combio/asignacion")]
        public string Razon { get; set; }

        #endregion


        #region obtener listado por tipo de usuario
        /// <summary>
        /// retorna una lista de administradores en base a su usuario
        /// </summary>
        /// <param name="context"></param>
        /// <param name="User"></param>
        /// <returns></returns>
        public static async System.Threading.Tasks.Task<List<AdmoHorario>> listadoPorTipoUsuario(ApplicationDbContext context, ClaimsPrincipal User) 
        {

            List<AdmoHorario> list = new();
            List<Empleado> empleados = await Empleado.EmpleadosXUsuarioLigth(context, User);
            if (empleados == null || empleados.Count < 1)
                return list;


            foreach (var emp in empleados)
            {
                var band = await context.AdmoHorarios.Where(x => x.Empleadoid == emp.id).ToListAsync();
                foreach (var e in band)
                    list.Add(e);
            }
            return list;
        }
        /// <summary>
        /// el listado completo
        /// </summary>
        /// <param name="context"></param>
        /// <param name="User"></param>
        /// <returns></returns>
        public static async System.Threading.Tasks.Task<List<AdmoHorario>> listadoPorTipoUsuarioComplete(ApplicationDbContext context, ClaimsPrincipal User)
        {

            List<AdmoHorario> list = new();
            List<Empleado> empleados = await Empleado.EmpleadosXUsuarioLigth(context, User);
            if (empleados == null || empleados.Count < 1)
                return list;


            foreach (var emp in empleados)
            {
                var band = await context.AdmoHorarios
                    .Include(x => x.Empleado)
                    .Include(x => x.Horario)
                    .Include(x => x.Admo)
                    .Where(x => x.Empleadoid == emp.id).ToListAsync();
                foreach (var e in band)
                    list.Add(e);
            }
            return list;
        }
        /// <summary>
        /// historico
        /// </summary>
        /// <param name="context"></param>
        /// <param name="User"></param>
        /// <returns></returns>

        public static async System.Threading.Tasks.Task<List<AdmoHorario>> listadoPorTipoUsuarioHistorico(ApplicationDbContext context, ClaimsPrincipal User)
        {

            List<AdmoHorario> list = new();
            List<Empleado> empleados = await Empleado.EmpleadosXUsuarioLigth(context, User);
            if (empleados == null || empleados.Count < 1)
                return list;


            foreach (var emp in empleados)
            {
                var band = await context.AdmoHorarios
                    .Include(x => x.Empleado).ThenInclude(x => x.user)
                    .Include(x => x.Horario)
                    .Where(x => x.Empleadoid == emp.id).ToListAsync();
                foreach (var e in band)
                    list.Add(e);
            }
            return list;
        }
        #endregion

    }
}
