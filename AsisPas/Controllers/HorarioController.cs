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
    /// controller for model Horario
    /// </summary>
    public class HorarioController : Controller
    {
        #region constructor

        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="context"></param>
        /// <param name="mapper"></param>
        public HorarioController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        #endregion


        #region listado
        /// <summary>
        /// vista inicial para Horarios
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {

            try
            {
                var ent = await context.Horarios.Where(x => x.act == true)
                    .Include(x => x.Empresa).ToListAsync();

                var ret = mapper.Map<List<HorarioDTO>>(ent);
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
        public async Task<IActionResult> Nueva()
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
        public async Task<IActionResult> Guardar(Horario ins)
        {
            try
            {
                ins.act = true;
                if(ins.sinDescanzo)
                {
                    ins.hbf = null;
                    ins.hbi = null;
                }

                context.Add(ins);
                await context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ViewBag.Err = "upss, parece que no tienes permisos para editar esta empresa";
                return RedirectToAction("Nueva", ins);
            }
        }

        #endregion

        #region editar
        /// <summary>
        /// para editar un elemento
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Editar(int id)
        {
            try
            {
                var model = await context.Horarios.FirstOrDefaultAsync(x => x.id == id);
                ViewBag.Empresas = Empresa.toSelect(await context.Empresas.Where(x => x.act == true).ToListAsync(), model.Empresaid);
                return View(model);
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
        public async Task<IActionResult> save(Horario ins)
        {
            try
            {
                var ent = await context.Horarios.FirstOrDefaultAsync(x => x.id == ins.id);
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
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                var model = await context.Horarios.FirstOrDefaultAsync(x => x.id == id);
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
