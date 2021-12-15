using AsisPas.Data;
using AsisPas.DTO;
using AsisPas.Entitys;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace AsisPas.Controllers
{
    /// <summary>
    /// para ver las marcas
    /// </summary>
    [Authorize]
    public class MarcaController : Controller
    {

        #region ctor
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;


        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="context"></param>
        /// <param name="mapper"></param>

        public MarcaController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }
        #endregion



        #region vista inicial
        /// <summary>
        /// la vista inicial indica el filtro para ver las marcas
        /// </summary>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<IActionResult> Index()
        {
            var marcas = await context.Marcaciones
                .Include(x => x.Empleado).ThenInclude(x => x.user)
                .Include(x => x.Sede)
                .Where(x => x.marca.ToString().Contains(DateTime.Now.ToString()))
                .ToListAsync();
            var empleados = await Empleado.EmpleadosXUsuario(context, User);
            ViewBag.Empleados = Empleado.toSelect(empleados);
            ViewBag.Marcar = marcas;
            ViewBag.fi = DateTime.Now;
            ViewBag.ff = DateTime.Now;

            return View();
        }

        /// <summary>
        /// para generar el resultado
        /// </summary>
        /// <param name="find"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<IActionResult> Busqueda(BusquedaRegistroPorUsuario find)
        {
            try
            {
                var marcas = await context.Marcaciones
               .Include(x => x.Empleado).ThenInclude(x => x.user)
               .Include(x => x.Sede)
               .Where(x => x.Empleadoid == find.Empleadoid && x.marca >= find.inicio && x.marca <= find.fin)
               .ToListAsync();
                var empleados = await Empleado.EmpleadosXUsuario(context, User);
                ViewBag.Empleados = Empleado.toSelect(empleados); 
                ViewBag.Marcar = marcas;
                ViewBag.fi = find.inicio;
                ViewBag.ff = find.fin;
                return View("Index");
            }catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return RedirectToAction("Index");
            }
        }
        #endregion
    }
}
