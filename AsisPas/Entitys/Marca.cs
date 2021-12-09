using AsisPas.Data;
using AsisPas.DTO;
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
    /// registra una marca echa
    /// </summary>
    public class Marca : MD5
    {

        #region props
        /// <summary>
        /// identificacion de la marca
        /// </summary>
        [Key]
        public int id { get; set; }
        /// <summary>
        /// la marca con hora y fecha
        /// </summary>
       [Required(ErrorMessage ="La marca es obligatoria")]
        public DateTime marca { get; set; }
        /// <summary>
        /// prop de nav
        /// </summary>
        public Sedes Sede { get; set; }
        /// <summary>
        /// sede en la que se realizo la marca
        /// </summary>
       [Required(ErrorMessage ="Se Necesita indicar donde se realizo la marca")]
        public int Sedeid { get; set; }
        /// <summary>
        /// codigo de puerta en caso de utilizarlo
        /// </summary>
        public string Gate { get; set; } = "NCode";
        /// <summary>
        /// prop nav
        /// </summary>
        public Empleado Empleado { get; set; }
        /// <summary>
        /// persona que realiza la marca
        /// </summary>
       [Required(ErrorMessage ="Quien hizo la marca")]
        public int Empleadoid { get; set; }
        /// <summary>
        /// prop nav
        /// </summary>
        public Horario Horario { get; set; }
        /// <summary>
        /// horario que tiene activo en este momento
        /// </summary>
       [Required(ErrorMessage ="Este dato es indispensable")]
        public int Horarioid { get; set; }
        /// <summary>
        /// indica tipo de ingreso
        /// 0 => inincio a jornada
        /// 1 => inicio descanso
        /// 2 => fin descanso
        /// 3 => fin de jornada
        /// </summary>
       [Required(ErrorMessage ="Indique el tipo de ingreso")]
        [Range(0,3)]
        public int TipoIngreso { get; set; }
        /// <summary>
        /// hash
        /// </summary>
        [Required]
        public string Hash { get; set; }
        /// <summary>
        /// key
        /// </summary>
        [Required]
        public string key { get; set; }
        #endregion

        #region complete hash
        /// <summary>
        /// para generar nuestros hash and key
        /// </summary>
        public void Complete()
        {

            this.key= GenerarKey();

            var sensitive = $"dia:{marca.ToString("dd/MM/yyyy")}||Hora:{marca.ToString("HH/mm/ss")}||MarcaTipo:{tipoIngreso()}||idEmpleado:{this.Empleadoid}";

            this.Hash = Encriptar(sensitive,this.key);
        }


        #endregion

        #region tipos de ingreso

        /// <summary>
        /// entrega en lenguaje el valor de tipo de ingreso
        /// </summary>
        /// <returns></returns>
        public string tipoIngreso()
        {
            switch (this.TipoIngreso)
            {
                case 0:
                    return "Inicio De Jornada";
                case 1:
                    return "Inicio del descanzo";
                case 2:
                    return "fin del descanzo";
                case 3:
                    return "Fin de la jornada";
                default:
                    return "";
            }
        }
        /// <summary>
        /// entrega los tipos de ingresos necesarios
        /// </summary>
        /// <returns></returns>
        public static List<TipoMarca> TiposIngresos()
        {
            List<TipoMarca> list = new();

            list.Add(new TipoMarca()
            {
                value = 0,
                desc = "Inicio De Jornada"
            });


            list.Add(new TipoMarca()
            {
                value = 1,
                desc = "Inicio del descanzo",
            });
            list.Add(new TipoMarca()
            {
                value = 2,
                desc = "fin del descanzo"
            });
            list.Add(new TipoMarca()
            {
                value = 3,
                desc = "Fin de la jornada"
            });

            return list;
           
        }

        /// <summary>
        /// ver tipo de ingreso
        /// </summary>
        /// <param name="tipoMarca"></param>
        /// <returns></returns>
        public static string VerTiposIngresos(int tipoMarca)
        {
            switch (tipoMarca)
            {
                case 0:
                    return "Inicio De Jornada";
                case 1:
                    return "Inicio del descanzo";
                case 2:
                    return "fin del descanzo";
                case 3:
                    return "Fin de la jornada";
                default:
                    return "";
            }


        }

        #endregion

        #region aux para vistas




        /// <summary>
        /// para retornar 1 elemento como select
        /// </summary>
        /// <param name="tipe"></param>
        /// <param name="select"></param>
        /// <returns></returns>
        public static SelectListItem toSelect( TipoMarca tipe, bool select)
        {
            return new SelectListItem()
            {
                Text = tipe.desc,
                Value = tipe.value.ToString(),
                Selected = select
            };
        }

        /// <summary>
        /// para el retorno en lista
        /// </summary>
        /// <returns></returns>
        public static List<SelectListItem> toSelect()
        {
            var emp = TiposIngresos();
            List<SelectListItem> ret = new(); 
            foreach (var item in emp)
                ret.Add(toSelect(item, false));
            ret.Add(new SelectListItem()
            {
                Text = "Seleccion El Tipo De Marca",
                Value = "",
                Selected = true
            });

            return ret;
        }
        #endregion

        #region asnync method
        /// <summary>
        /// para obtener las 4 marcas del usuario
        /// </summary>
        /// <param name="idEmpleado"></param>
        /// <param name="fecha"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static async System.Threading.Tasks.Task<List<Marca>> MarcasDiaYUsuario(int idEmpleado, DateTime fecha, ApplicationDbContext context)
        {
            try
            {
                var marcas = await context.Marcaciones.Where(x => x.Empleadoid == idEmpleado
                && x.marca.ToString("dd/MM/yyyy") == fecha.ToString("dd/MM/yyyy")).ToListAsync();
                var ini = marcas.Where(x => x.TipoIngreso == 0).FirstOrDefault();
                var end = marcas.Where(x => x.TipoIngreso == 3).FirstOrDefault();
                var iniD = marcas.Where(x => x.TipoIngreso == 1).FirstOrDefault();
                var endD = marcas.Where(x => x.TipoIngreso == 2).FirstOrDefault();

                return new() { ini, end, iniD, endD };


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }

        }


            #endregion
     
    }



}
