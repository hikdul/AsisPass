using AsisPas.Data;
using AsisPas.Helpers;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;

namespace AsisPas.Entitys
{
    /// <summary>
    /// clase para almocenar permisos y otros elementos
    /// permisos medicos, vacaciones, etc...
    /// </summary>
    public class Permisos: Iid, IAct
    {

        #region props
        /// <summary>
        /// id
        /// </summary>
        [Key]
        public int id { get; set; }
        /// <summary>
        /// act
        /// </summary>
        public bool act { get; set; } = true;
        /// <summary>
        /// Empleado al que se le otorga el permiso
        /// </summary>
        [Required(ErrorMessage ="Un Epleado es necesario")]
        public int Empleadoid { get; set; }
        /// <summary>
        /// prop de navegacion
        /// </summary>
        public Empleado Empleado { get; set; }
        /// <summary>
        /// descrigcion del permiso
        /// </summary>
        [Required(ErrorMessage ="explique brevemente el permiso otorgado")]
        [StringLength(100,ErrorMessage ="breve son maximo 100 caracteres")]
        [MinLength(5, ErrorMessage ="Al menos agrege 5 caracteres como explicacion")]
        public string Desc { get; set; }
        /// <summary>
        /// alguna imagen que desee acompañar
        /// </summary>
        public string PruebaFotografica { get; set; }
        /// <summary>
        /// inicio del periodo del permiso
        /// </summary>
        [Required(ErrorMessage ="indique la fecha de inicio")]
        public DateTime inicio{ get; set; }
        /// <summary>
        /// fin del periodo del permiso
        /// </summary>
        [Required(ErrorMessage ="Indique la fecha en que acaba")]
        public DateTime fin{ get; set; }
        /// <summary>
        /// quien aprueba el permiso
        /// </summary>
        public int AprobadoPorid { get; set; }
        /// <summary>
        /// prop nav
        /// </summary>
        public AdmoEmpresas AprobadoPor { get; set; }
        #endregion

        #region validate
            
        /// <summary>
        /// valida que el periodo sea correcto
        /// </summary>
        /// <returns></returns>
        public bool validate()
        {
            return this.inicio <= this.fin;
        }
        #endregion

        #region listados

        /// <summary>
        /// para obtener el listado de los permisos de los ultimo 30 dias
        /// </summary>
        /// <param name="context"></param>
        /// <param name="User"></param>
        /// <returns></returns>
        public static async System.Threading.Tasks.Task<List<Permisos>> listadolastMont(ApplicationDbContext context, ClaimsPrincipal User)
        {
            List<Permisos> list = new();
            try
            {
                var date = DateTime.Now.AddDays(-30);
                var emp = await Empleado.EmpleadosXUsuarioLigth(context, User);

                foreach (var e in emp)
                {
                    var flag = await context.Permisos
                        .Include(x => x.Empleado).ThenInclude(x => x.user)
                        .Include(x => x.AprobadoPor).ThenInclude(x => x.user)
                        .Where(x => x.Empleadoid == e.id && x.inicio > date)
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

        #endregion

    }
}
