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
    /// vistas de empresas
    /// </summary>
    public class EmpresaController : Controller
    {

        #region contructor
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="context"></param>
        /// <param name="mapper"></param>
        public EmpresaController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }
        #endregion

        #region vista inincial
        /// <summary>
        /// vista inicial
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            try
            {
                var model = await context.Empresas.Where(x => x.act).ToListAsync();
                return View(mapper.Map<List<EmpresaDTO>>(model));
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
        public IActionResult Editar()
        {
            try
            {
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
        public async Task<IActionResult> Guardar(empresaDTO_in ins)
        {
            try { 

                context.Add(mapper.Map<Empresa>(ins));
                await context.SaveChangesAsync();
                return RedirectToAction("Index");
            
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ViewBag.Err = "upss, parece que no tienes permisos para editar esta empresa";
                return View("Editar",ins);
            }
        }

        #endregion

        #region editar
        /// <summary>
        /// para editar un elemento
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var model = await context.Empresas.FirstOrDefaultAsync(x => x.id == id);
                return View(mapper.Map<EmpresaDTO>(model));
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
        public async Task<IActionResult> save(EmpresaDTO ins)
        {
            try
            {
                var ent = await context.Empresas.FirstOrDefaultAsync(x => x.id == ins.id);
                ent = mapper.Map(ins,ent);
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
                var model = await context.Empresas.FirstOrDefaultAsync(x => x.id == id);
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
