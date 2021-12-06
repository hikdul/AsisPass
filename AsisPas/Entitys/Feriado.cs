using AsisPas.Data;
using AsisPas.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;

namespace AsisPas.Entitys
{
    /// <summary>
    /// feriados
    /// </summary>
    public class Feriado : Iid,IAct
    {
        #region props
        /// <summary>
        /// id
        /// </summary>
        [Key]
        public int id { get; set; }
        /// <summary>
        /// descripcion o contesto histarico
        /// </summary>
        [Required(ErrorMessage ="agrege una desc del dia")]
        [StringLength(50)]
        public string desc { get; set; }
        /// <summary>
        /// en el caso de feriados que se declaran como nuevo dia
        /// </summary>
        public bool UnaSolaVes { get; set; } = false;
        /// <summary>
        /// fecha en la que se aplica el feriado
        /// </summary>
        [Required(ErrorMessage ="La Fecha es importante")]
        public DateTime fecha { get; set; }
        /// <summary>
        /// active
        /// </summary>
        public bool act { get; set; }

        #endregion

        #region listado


        public static async System.Threading.Tasks.Task<List<Feriado>> listado(ApplicationDbContext context)
        {

            List<Feriado> list = new();
            try
            {

                var feriados = await context.Feriados.Where(x => x.act == true).ToListAsync();
                foreach (var item in feriados)
                {
                    if(!item.UnaSolaVes)
                        list.Add(item);
                    else
                        if (item.fecha.Year == DateTime.Now.Year)
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

        #endregion

    }
}
