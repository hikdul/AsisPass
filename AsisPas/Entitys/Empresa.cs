

using AsisPas.Helpers;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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
    }
}
