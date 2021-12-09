using AsisPas.Data;
using AsisPas.Entitys;
using AsisPas.Reportes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace AsisPas.Controllers
{
    /// <summary>
    /// controlado para generar reportes por asistencia
    /// </summary>
    public class ReporteAsistenciaController : Controller
    {
        #region ctor
        private readonly ApplicationDbContext context;
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="context"></param>
        public ReporteAsistenciaController(ApplicationDbContext context)
        {
            this.context = context;
        }
        #endregion


        #region buscador
        /// <summary>
        /// esta vista carga el buscador y sus elementos
        /// </summary>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<IActionResult> Index()
        {
            try
            {
                List<ElementosBusqueda> elementos = new();
                SelectListItem empty = new()
                {
                    Value = "-1",
                    Selected = true,
                    Text = "-- ### Todo- ### --"
                };

                var turnos = Horario.toSelect(await Horario.ListadoPorUsuario(context, User));
                var faenas = Sedes.toSelect(await Sedes.ListadoPorUsuario(context, User));
                turnos.Add(empty);
                faenas.Add(empty);

                ViewBag.Turnos = turnos;
                ViewBag.Faenas = faenas;

                var empleados = await Empleado.EmpleadosXUsuarioLigth(context, User);
                foreach (var item in empleados)
                {
                    var elemento = new ElementosBusqueda();
                    await elemento.Up(item.id, context);
                    elementos.Add(elemento);
                }

                ViewBag.Empleados = elementos;

                return View();

            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ViewBag.Err = "Hubo un problema al cargar los datos, Por Favor intente mas tarde.";
                return View();
            }
        }

        #endregion

        #region generar reporte
        /// <summary>
        /// para generar el listado de reportes
        /// </summary>
        /// <param name="ins"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<IActionResult> Generar(Finder ins)
        {
            try
            {
                List<ReporteAsistencia> list = new();
                foreach (var id in ins.Empleadoids)
                {
                    ReporteAsistencia flag = new();
                    await flag.Up(id,ins.inicio,ins.fin,context);
                    list.Add(flag);
                }
                ViewBag.Reportes = list;
                return View();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return View();
            }
        }


        #endregion
    }
}
