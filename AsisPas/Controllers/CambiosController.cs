using AsisPas.Data;
using AsisPas.DTO;
using AsisPas.Entitys;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AsisPas.Controllers
{
    /// <summary>
    /// para generar los cambios
    /// </summary>
    public class CambiosController : Controller
    {

        #region constructor
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;


        public CambiosController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        #endregion

        #region vista inicial
        /// <summary>
        /// para ver el listado de cambios
        /// </summary>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<IActionResult> Index()
        {
            try
            {
                var users = await Empleado.EmpleadosXUsuario(context, User);

                List<Cambios> list = new List<Cambios>();

                foreach (var us in users)
                {
                    var band = await context.Cambios
                        .Include(b => b.Empleado).ThenInclude(x => x.user)
                        .Where(x => x.Empleadoid == us.id && x.fecha > DateTime.Now.AddDays(-15))
                        .ToListAsync();
                    foreach (var item in band)
                        list.Add(item);

                }

                return View(list);

            }catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ViewBag.Err = "Algo Salio mal, Intente mas tarde...";
                return View();
            }
        }

        #endregion

        #region crear un cambio

        /// <summary>
        /// vista para crear un nuevo cambio
        /// </summary>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<IActionResult> Nuevo()
        {
            try
            {
                ViewBag.Empleados = Empleado.toSelect(await Empleado.EmpleadosXUsuario(context, User));
                ViewBag.TiposMarca = Marca.toSelect();


                return View();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ViewBag.Err = "Algo Salio mal, Intente mas tarde...";
                return View();
            }
        }

        /// <summary>
        /// para agregar un nuevo cambio
        /// </summary>
        /// <param name="ins"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<IActionResult> Crear(CambiosDTO_in ins)
        {
            try
            {
                var ent = mapper.Map<Cambios>(ins);

                context.Add(ent);
                await context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ViewBag.Err = "Algo Salio mal, Intente mas tarde...";
                ViewBag.Empleados = Empleado.toSelect(await Empleado.EmpleadosXUsuario(context, User));
                ViewBag.TiposMarca = Marca.toSelect();
                return View("Nuevo",ins);
            }
        }

        #endregion
    }
}
