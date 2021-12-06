using AsisPas.Data;
using AsisPas.Helpers;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AsisPas.Entitys
{
    /// <summary>
    /// mantiene los datos de empleados
    /// </summary>
    public class Empleado : Iid, IAct
    {
        #region propiedades
        /// <summary>
        /// id del empleado
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// si se encuentra activo
        /// </summary>
        public bool act { get; set; }
        /// <summary>
        /// usuario que tiene el ros
        /// </summary>
        public int userid { get; set; }
        /// <summary>
        /// prop de nav
        /// </summary>
        public Usuario user { get; set; }
        /// <summary>
        /// empresa en la que esta a cargo
        /// </summary>
        public int Empresaid { get; set; }
        /// <summary>
        /// prop de nav
        /// </summary>
        public Empresa Empresa { get; set; }


        #endregion

        #region obtener empleados por usuario
        /// <summary>
        /// para obtener una lista de empleados por empresa
        /// </summary>
        /// <param name="context"></param>
        /// <param name="User"></param>
        /// <returns></returns>
        public static async System.Threading.Tasks.Task<List<Empleado>> EmpleadosXUsuarioLigth(ApplicationDbContext context, ClaimsPrincipal User)
        {
            List<Empleado> list = new();

            if (User.IsInRole("Empleado"))
            {
                list.Add(await EmpleadosXEmail(User.Identity.Name, context, User));
                return list;
            }

            var empresas = await Empresa.FiltrarEmpresas(context, User);

            if (empresas == null || empresas.Count < 1)
                return list;

            foreach (var item in empresas)
            {
                var band = await context.Empleados.Where(x => x.Empresaid == item.id).ToListAsync();
                foreach (var emp in band)
                    list.Add(emp);
            }

            return list;
        }
        /// <summary>
        /// obtiene todos los datos des empleados
        /// </summary>
        /// <param name="context"></param>
        /// <param name="User"></param>
        /// <returns></returns>
        public static async System.Threading.Tasks.Task<List<Empleado>> EmpleadosXUsuario(ApplicationDbContext context, ClaimsPrincipal User)
        {
            List<Empleado> list = new();

            if (User.IsInRole("Empleado")) {
                list.Add(await EmpleadosXEmail(User.Identity.Name, context, User));
                return list; 
            }

            var empresas = await Empresa.FiltrarEmpresas(context, User);

            if (empresas == null || empresas.Count < 1)
                return list;

            foreach (var item in empresas)
            {
                var band = await context.Empleados.Include(x => x.user).Where(x => x.Empresaid == item.id).ToListAsync();
                foreach (var emp in band)
                    list.Add(emp);
            }

            return list;
        }
        /// <summary>
        /// para obtener los datos de un empleado por medio de su email
        /// </summary>
        /// <param name="Email"></param>
        /// <param name="context"></param>
        /// <param name="User"></param>
        /// <returns></returns>
        public static async System.Threading.Tasks.Task<Empleado> EmpleadosXEmail(string Email,ApplicationDbContext context, ClaimsPrincipal User)
        {
            return await context.Empleados.Include(x => x.user).Where(x => x.user.Email == Email).FirstOrDefaultAsync();
        }

        #endregion

        #region aux para vistas

        /// <summary>
        /// para retornar 1 elemento como select
        /// </summary>
        /// <param name="emp"></param>
        /// <param name="select"></param>
        /// <returns></returns>
        public static SelectListItem toSelect(Empleado emp, bool select)
        {
            if(emp.user != null)
            return new SelectListItem()
            {
                Text = $"{emp.user.Nombres} {emp.user.Apellidos} {emp.user.Rut}",
                Value = emp.id.ToString(),
                Selected = select
            };
            return new();
        }

        /// <summary>
        /// para el retorno en lista
        /// </summary>
        /// <param name="emp"></param>
        /// <param name="idSelect"></param>
        /// <returns></returns>
        public static List<SelectListItem> toSelect(List<Empleado> emp, int idSelect = 0)
        {
            List<SelectListItem> ret = new();
            
            foreach (var item in emp)
                ret.Add(toSelect(item, item.id == idSelect));
            return ret;
        }
        #endregion
    }
}
