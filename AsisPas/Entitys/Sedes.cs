using AsisPas.Data;
using AsisPas.Helpers;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace AsisPas.Entitys
{
    /// <summary>
    /// para implementar los lugares de trabajo de cada empresa
    /// </summary>
    public class Sedes: Iid,IAct
    {
        #region region
        /// <summary>
        /// id
        /// </summary>
        [Key]
        public int id { get; set; }
        /// <summary>
        /// nombre de la sede
        /// </summary>
        [Required(ErrorMessage ="Un Nombre es obligatorio")]
        [StringLength(50)]
        public string Nombre { get; set; }

        /// <summary>
        /// direccion de donde se ubica la sede
        /// </summary>
        [Required(ErrorMessage ="La direccion es vital")]
        [StringLength(100)]
        public string Direccion { get; set; }
        /// <summary>
        /// empresa a la que pertenece la sede
        /// </summary>
        [Required(ErrorMessage ="La Sede necesita su empresa de destino")]
        public int  Empresaid { get; set; }
        /// <summary>
        /// prop nav empresa
        /// </summary>
        public Empresa Empresa { get; set; }

        // ++++++
        // esto datos estan alli por si algun dia los agregamos
        // ++++++

        /// <summary>
        /// latitud
        /// </summary>
        public double lat { get; set; }
        /// <summary>
        /// longitud
        /// </summary>
        public double lng { get; set; }
        /// <summary>
        /// si se encuentra activo o no
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
        public static SelectListItem toSelect(Sedes emp, bool select)
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
        public static List<SelectListItem> toSelect(List<Sedes> emp, int idSelect = 0)
        {
            List<SelectListItem> ret = new();
            foreach (var item in emp)
                ret.Add(toSelect(item, item.id == idSelect));
            return ret;
        }
        #endregion

    }
}
