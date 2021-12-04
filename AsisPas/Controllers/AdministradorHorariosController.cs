using AsisPas.Data;
using AsisPas.DTO;
using AsisPas.Entitys;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AsisPas.Controllers
{
    /// <summary>
    /// para manipular los datos del controlador de horarios
    /// </summary>
    public class AdministradorHorariosController : Controller
    {

        #region ctor
        // inyecciones
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;


        /// <summary>
        /// ctor
        /// </summary>
        public AdministradorHorariosController(ApplicationDbContext context, IMapper mapper )
        {
            this.context = context;
            this.mapper = mapper;
        }

        #endregion


        #region vistas listados
        /// <summary>
        /// listado de los que se encuentran realizados
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            var model = await AsignadorDeHorarios.VerListado(context, User);
            return View(model);
        }

        /// <summary>
        /// para ver el historial de los admo asignados
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Historico()
        {
            var model = await AdmoHorario.listadoPorTipoUsuarioHistorico(context, User);
            return View(model);
        }

        #endregion
    }
}
