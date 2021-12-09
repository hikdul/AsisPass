using AsisPas.Data;
using AsisPas.DTO;
using AsisPas.Entitys;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AsisPas.Controllers
{
    /// <summary>
    /// controller para sedes
    /// </summary>
    public class SedesController : Controller
    {
        #region ctor

        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        /// <summary>
        /// ctor
        /// </summary>
        public SedesController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }
        #endregion


        #region vista inicial
        /// <summary>
        /// index
        /// </summary>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<IActionResult> Index()
        {
            try
            {
                var ent = await Sedes.ListadoPorUsuario(context, User);
                var ret = mapper.Map<List<SedeDTO>>(ent);
                return View(ret);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ViewBag.Err = "Error al cargar los datos, Intente luego";
                return View();
            }
        }


        #endregion

        #region crear
        /// <summary>
        /// vista para editar, crear una empresa
        /// </summary>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<IActionResult> Nueva()
        {
            try
            {
                ViewBag.Empresas = Empresa.toSelect(await context.Empresas.Where(x => x.act == true).ToListAsync());
                return View();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ViewBag.Err = "upss, parece que no tienes permisos para editar esta empresa";
                return View();
            }


        }
        /// <summary>
        /// accion de almacenar los datos en la base de datos
        /// </summary>
        /// <param name="ins"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<IActionResult> Guardar(SedeDTO_in ins)
        {
            try
            {
                context.Add(mapper.Map<Sedes>(ins));
                await context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ViewBag.Err = "upss, parece que no tienes permisos para editar esta empresa";
                return View("Editar", ins);
            }
        }

        #endregion

        #region editar
        /// <summary>
        /// para editar un elemento
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<IActionResult> Editar(int id)
        {
            try
            {
                var model = await context.Sedes.FirstOrDefaultAsync(x => x.id == id);
                ViewBag.Empresas = Empresa.toSelect(await context.Empresas.Where(x => x.act == true).ToListAsync(), model.Empresaid);
                return View(mapper.Map<SedeDTO_up>(model));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ViewBag.Err = "upss, parece que no tienes permisos para editar esta empresa";
                return View();
            }


        }
        /// <summary>
        /// accion de almacenar los datos en la base de datos
        /// </summary>
        /// <param name="ins"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<IActionResult> save(SedeDTO_up ins)
        {
            try
            {
                var ent = await context.Sedes.FirstOrDefaultAsync(x => x.id == ins.id);
                ent = mapper.Map(ins, ent);
                ent.act = true;
                await context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ViewBag.Err = "upss, parece que no tienes permisos para editar esta empresa";
                return View("Editar", ins);
            }
        }

        #endregion

        #region delete
        /// <summary>
        /// para eliminar un elemento
        /// </summary>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<IActionResult> Eliminar(int id)
        {
            try
            {
                var model = await context.Sedes.FirstOrDefaultAsync(x => x.id == id);
                model.act = false;
                await context.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ViewBag.Err = "Error al cargar los datos, Intente luego";
                return RedirectToAction("Index");
            }
        }

        #endregion
    }
}
