using AsisPas.Data;
using AsisPas.DTO;
using AsisPas.Entitys;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace AsisPas.Controllers
{
    /// <summary>
    /// controlador de vistas de incidencias
    /// </summary>
    [Authorize]
    public class IncidenciasController : Controller
    {
        #region ctor
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="context"></param>
        /// <param name="mapper"></param>
        public IncidenciasController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }
        #endregion


        #region vista inicial

        /// <summary>
        /// para ver mi listado de incidencias
        /// </summary>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<IActionResult> Index()
        {
            try
            {
                return View(await Incidencia.ListadoXUsuarioLast15Days(context,User));
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                return View();
            }
        }

        #endregion


        #region crear incidencia
        /// <summary>
        /// vista para crear una incidencia
        /// </summary>
        /// <returns></returns>
        public IActionResult Crear()
        {

            try
            {
                ViewBag.TiposMarca = Marca.toSelect();  
                return View();
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                return View();
            }
        }

        /// <summary>
        /// guardar la nueva incidencia
        /// </summary>
        /// <param name="ins"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<IActionResult> Guardar(IncidenciaDTO_in ins)
        {
            try
            {
                var emp = await Empleado.EmpleadosXEmail(User.Identity.Name, context, User);
                if (emp == null || emp.id < 1)
                {
                    ViewBag.Err = "Parece Que No Tienes Permisos para ejecutar esta accion...";
                    return View("Index",await Incidencia.ListadoXUsuarioLast15Days(context, User));
                }

                var ent = mapper.Map<Incidencia>(ins);
                ent.Empleado = emp;
                ent.Empleadoid = emp.id;
                
                context.Add(ent);
                await context.SaveChangesAsync();

                return View("Index");
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                return View("Index",await Incidencia.ListadoXUsuarioLast15Days(context, User));
            }
        }

        #endregion
    }
}
