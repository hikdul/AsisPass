using AsisPas.Data;
using AsisPas.DTO;
using AsisPas.Entitys;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AsisPas.Controllers
{
    /// <summary>
    /// para administrar los puntos de acceso
    /// </summary>
    public class GatesController : Controller
    {
        #region ctor
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="context"></param>
        /// <param name="mapper"></param>
        public GatesController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }
        #endregion


        #region listado inicial
        /// <summary>
        /// vista del listado inicial
        /// </summary>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<IActionResult> Index()
        {
            try
            {
                return View(await context.Gates.Include(x => x.Sede).ThenInclude(x => x.Empresa).ToListAsync());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return View();
            }
        }


        #endregion

        #region crear gate

        /// <summary>
        /// vista para crear una nueva puerta
        /// </summary>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<IActionResult> Crear()
        {
            try
            {
                var empresas = await Empresa.FiltrarEmpresas(context, User);
                ViewBag.EmpresasData = empresas; 
                var list = Empresa.toSelect(empresas);
                list.Add(new()
                {
                    Text = " === SELECCIONE UNA EMPRESA ===",
                    Value = "",
                    Selected = true,
                });
                ViewBag.Empresas = list;
                var sedes = await context.Sedes.Where(x => x.act == true).ToListAsync();
                ViewBag.SedesData = sedes;
                return View();
            }
            catch (Exception)
            {
                return RedirectToAction("logout", "Cuentas");
            }

        }

        /// <summary>
        /// para almacenar los datos
        /// </summary>
        /// <param name="ins"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<IActionResult> Guardar(GateDTO_in ins)
        {
            try
            {
                Gate ent = new();
                ent.Sedeid = ins.Sedeid;
                ent.Desc = ins.Desc == null ? "": ins.Desc;
                ent.act = true;
                ent.complete();
                context.Add(ent);
                await context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                ViewBag.Err = "upss, algo salio mal. intente mas tarde";
                return View("Index");
            }
        }

        /// <summary>
        /// para desactivar un elemento
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<IActionResult> Eliminar(int id)
        {
            try
            {
                Gate ent = await context.Gates.Where(x => x.id == id).FirstOrDefaultAsync();
                if(ent == null)
                {
                    ViewBag.Err = "upss, algo salio mal. intente mas tarde";
                    return View("Index");
                }
                ent.act = false;
                await context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                ViewBag.Err = "upss, algo salio mal. intente mas tarde";
                return View("Index");
            }
        }

        /// <summary>
        /// para activar un elemento
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<IActionResult> Activar(int id)
        {
            try
            {
                Gate ent = await context.Gates.Where(x => x.id == id).FirstOrDefaultAsync();
                if (ent == null)
                {
                    ViewBag.Err = "upss, algo salio mal. intente mas tarde";
                    return View("Index");
                }
                ent.act = true;
                await context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                ViewBag.Err = "upss, algo salio mal. intente mas tarde";
                return View("Index");
            }
        }
        #endregion
    }
}
