using AsisPas.Data;
using AsisPas.DTO;
using AsisPas.Entitys;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace AsisPas.Controllers
{
    /// <summary>
    /// para generar los permisos y/o vacaciones
    /// </summary>
    public class PermisosController : Controller
    {
        #region ctor

        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="context"></param>
        /// <param name="mapper"></param>
        public PermisosController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        #endregion


        #region vista inicial

        /// <summary>
        /// vista inicial
        /// </summary>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<IActionResult> Index()
        {
            try
            {
                var model = await Permisos.listadolastMont(context, User);
                return View(model);
            }
            catch (Exception)
            {
                ViewBag.Err = "Error al Cargar Data, intente mas tarde";
                return View();
            }
        }
        #endregion

        #region Crear Permiso

        /// <summary>
        /// vista para crear un nuevo permisos
        /// </summary>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<IActionResult> Crear()
        {
            try
            {
                ViewBag.Empleados = Empleado.toSelect(await Empleado.EmpleadosXUsuario(context, User));
                return View();
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                ViewBag.Err = "Error al Cargar Data, intente mas tarde";
                return View();
            }

        }


        /// <summary>
        /// vista para crear un nuevo permisos
        /// </summary>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<IActionResult> Guardar(PermisosDTO_in ins)
        {
            try
            {
                if(!User.IsInRole("Empresa"))
                    return RedirectToAction("logout", "cuentas");

                var admo = await context.AdmoEmpresas.Where(x => x.user.Email == User.Identity.Name && x.act).FirstOrDefaultAsync();
                
                if(admo == null || admo.id < 1)
                    return RedirectToAction("logout", "cuentas");

                var ent = mapper.Map<Permisos>(ins);
                ent.AprobadoPor = admo;
                ent.AprobadoPorid = admo.id;

                context.Add(ent);
                await context.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                
                return RedirectToAction("logout","cuentas");
            }

        }

        #endregion

    }
}
