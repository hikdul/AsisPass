using AsisPas.Data;
using AsisPas.DTO;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
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

        #region vistas
        /// <summary>
        /// vista inicial
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            try
            {
                var model = await context.Empresas.ToListAsync();
                return View(mapper.Map<EmpresaDTO>(model));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ViewBag.Err = "Error al cargar los datos, Intente luego";
                return View();
            }
        }


        #endregion

    }
}
