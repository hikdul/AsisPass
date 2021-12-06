using AsisPas.Data;
using AsisPas.DTO;
using AsisPas.Entitys;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AsisPas.Controllers
{
    /// <summary>
    /// Feriados
    /// </summary>
    public class FeriadoController : Controller
    {
        #region ctor
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="context"></param>
        /// <param name="mapper"></param>
        public FeriadoController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        #endregion

        #region vista inicial
        /// <summary>
        /// para mostrar la lista de feriados activos
        /// </summary>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<IActionResult> Index()
        {
            try
            {
                return View(await Feriado.listado(context));
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine(ex.Message);
                ViewBag.Err = "Error al cargar los datos";
                return View();
            }
        }

        #endregion


        #region Crear

        /// <summary>
        /// vista para crear un nuevo feriado
        /// </summary>
        /// <returns></returns>
        public IActionResult Nuevo()
        {
            try
            {
                return View();
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine(ex.Message);
                return View();
            }
        }

        /// <summary>
        /// para guardar un nuevo elemento
        /// </summary>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<IActionResult> Guardar(FeriadoDTO_in ins)
        {
            try
            {
                var ent = mapper.Map<Feriado>(ins); 
                context.Add(ent);
                await context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine(ex.Message);
                ViewBag.Err = "Algo salio mal intente nuevamente";
                return View(ins);
            }
        }

        #endregion
    }
}
