

using AsisPas.Data;
using AsisPas.Helpers;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AsisPas.Entitys
{
    /// <summary>
    /// entidad para empresas
    /// </summary>
    public class Empresa : Iid,IAct
    {
        #region propiedades
        /// <summary>
        /// key
        /// </summary>
        [Key]
        public int id { get; set; }
        /// <summary>
        /// nombre de la empresa
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Nombre { get; set; }
        /// <summary>
        /// rut de la empresa
        /// </summary>
        [Required(ErrorMessage ="el Rut es necesario")]
        [StringLength(12)]
        public string Rut { get; set; }
        /// <summary>
        /// rubro en el que participa
        /// </summary>
        [StringLength(30)]
        public string Rubro { get; set; }
        /// <summary>
        /// logo de la empresa
        /// </summary>
        public string Logo { get; set; }
        /// <summary>
        /// si la empresa se encuentra activa o no
        /// </summary>
        public bool act { get; set; }
        #endregion


        #region aux para vistas

        /// <summary>
        /// para retornar 1 elemento como select
        /// </summary>
        /// <param name="emp"></param>
        /// <param name="select"></param>
        /// <returns></returns>
        public static SelectListItem toSelect(Empresa emp, bool select)
        {
            return new SelectListItem()
            {
                Text = emp.Nombre,
                Value = emp.id.ToString(),
                Selected = select
            };
        }

        /// <summary>
        /// para el retorno en lista
        /// </summary>
        /// <param name="emp"></param>
        /// <param name="idSelect"></param>
        /// <returns></returns>
        public static List<SelectListItem> toSelect(List<Empresa> emp, int idSelect = 0)
        {
            List<SelectListItem> ret = new();
            foreach (var item in emp)
                ret.Add(toSelect(item, item.id == idSelect));
            return ret;
        }
        #endregion

        #region filto por usuario
        /// <summary>
        /// para generar todo el filtro en un solo punto
        /// </summary>
        /// <param name="context"></param>
        /// <param name="User"></param>
        /// <returns></returns>
        public static async Task<List<Empresa>> FiltrarEmpresas(ApplicationDbContext context, ClaimsPrincipal User)
        {
            List<Empresa> empresas = new();
            if (User == null)
                return empresas;
            if (User.IsInRole("SuperAdmin"))
                return await context.Empresas.Where(x => x.act == true).ToListAsync();
            if(User.IsInRole("Empresa"))
            {
                var emp = await context.AdmoEmpresas.Where(x => x.user.Email == User.Identity.Name).FirstOrDefaultAsync();
                if (emp != null)
                    empresas.Add(await context.Empresas.FirstOrDefaultAsync(x => x.id == emp.Empresaid));
                return empresas;
            }
            if (User.IsInRole("Fiscal"))
            {
                var emp = await context.Fiscales.Where(x => x.user.Email == User.Identity.Name).FirstOrDefaultAsync();
                if (emp != null)
                    empresas.Add(await context.Empresas.FirstOrDefaultAsync(x => x.id == emp.Empresaid));
                return empresas;
            }
            empresas.Clear();
            return empresas;
        }


        #endregion
    }
}
