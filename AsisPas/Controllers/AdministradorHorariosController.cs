using AsisPas.Data;
using AsisPas.DTO;
using AsisPas.Entitys;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AsisPas.Controllers
{
    /// <summary>
    /// para manipular los datos del controlador de horarios
    /// </summary>
    [Authorize]
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
        public async System.Threading.Tasks.Task<IActionResult> Index()
        {
            var model = await AsignadorDeHorarios.VerListado(context, User);
            return View(model);
        }

        /// <summary>
        /// para ver el historial de los admo asignados
        /// </summary>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<IActionResult> Historico()
        {
            var model = await AdmoHorario.listadoPorTipoUsuarioHistorico(context, User);
            return View(model);
        }

        #endregion


        #region crear admo
        /// <summary>
        /// vista para crear un nuevo administrador
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<IActionResult> Crear(int id = 0)
        {
            var emp = await Empleado.EmpleadosXUsuario(context, User);
            emp = emp.Where(x => x.Articulo22 == false).ToList();
            ViewBag.Empleados = Empleado.toSelect(emp,id);
            ViewBag.Horarios = Horario.toSelect(await Horario.ListadoPorUsuario(context, User));

            return View();
        }

        /// <summary>
        /// para guardar el nuevo admo
        /// </summary>
        /// <param name="ins"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<IActionResult> Guardar(AdmoHorarioDTO_in ins)
        {
            try
            {
                if (await NewValid(ins.Empleadoid,ins.inicio, ins.fin))
                {

                    var ent = mapper.Map<AdmoHorario>(ins);

                    AdmoEmpresas cambiador = await context.AdmoEmpresas
                        .Where(x => x.user.Email == User.Identity.Name)
                        .FirstOrDefaultAsync();
                    if (cambiador == null)
                    {
                        ViewBag.Err = "Upss Parece que no tienes Permisos para ejecutar esta accion.";
                        var model = await AsignadorDeHorarios.VerListado(context, User);
                        return View("Index",model);
                    }

                    ent.Admo = cambiador;
                    ent.Admoid = cambiador.id;
                    ent.fechaAsignacion = DateTime.Now;

                    context.Add(ent);
                    await context.SaveChangesAsync();

                    return RedirectToAction("Index");
                }
                ViewBag.Err = "Algo anda mal con tus fechas";
                ViewBag.Empleados = Empleado.toSelect(await Empleado.EmpleadosXUsuario(context, User));
                ViewBag.Horarios = Horario.toSelect(await Horario.ListadoPorUsuario(context, User));

                return View("Crear", ins);

            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);

                ViewBag.Err = "Algo no esta bien en los datos ingresados, por favor verifique";
                ViewBag.Empleados = Empleado.toSelect(await Empleado.EmpleadosXUsuario(context, User));
                ViewBag.Horarios = Horario.toSelect(await Horario.ListadoPorUsuario(context, User));

                return View("Crear",ins);
            }
        }

        /// <summary>
        /// verifica si ne existe algo que no funcione
        /// </summary>
        /// <param name="idEmpleado"></param>
        /// <param name="inicio"></param>
        /// <param name="fin"></param>
        /// <returns></returns>
        private async System.Threading.Tasks.Task<bool> NewValid(int idEmpleado, DateTime inicio, DateTime fin)
        {
            if (inicio > fin)
                return false;

            var ent = await context.AdmoHorarios
                .Where( x => x.Empleadoid == idEmpleado &&( x.fin == inicio || x.inicio == fin || (x.inicio > inicio && x.fin < fin)))
                .ToListAsync();
            return ent.Count < 1;
        }
        #endregion




        #region editar
        /// <summary>
        /// vista para crear un nuevo administrador
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<IActionResult> Editar(int id = 0)
        {
            var admo = await context.AdmoHorarios
                .Include(x => x.Empleado).ThenInclude(x => x.user)
                .FirstOrDefaultAsync(x => x.id == id);
            if (admo == null)
                return View("Crear");

            var model = mapper.Map<AdmoHorarioDTO_up>(admo);
            List<Empleado> list = new();
            list.Add(admo.Empleado);
            ViewBag.Empleados = Empleado.toSelect(list, admo.Empleadoid);
            ViewBag.Horarios = Horario.toSelect(await Horario.ListadoPorUsuario(context, User));

            return View(model);
        }
        /// <summary>
        /// para almacenar los nuevos datos de las modificacion
        /// </summary>
        /// <param name="ins"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<IActionResult> Edit(AdmoHorarioDTO_up ins)
        {
            try
            {
                if (await validarEdit(ins.id,ins.Empleadoid,ins.inicio, ins.fin)) { 
                    //var ent = mapper.Map<AdmoHorario>(ins);

                    AdmoEmpresas cambiador = await context.AdmoEmpresas
                        .Where(x => x.user.Email == User.Identity.Name)
                        .FirstOrDefaultAsync();
                    if (cambiador == null)
                    {
                        ViewBag.Err = "Upss Parece que no tienes Permisos para ejecutar esta accion.";
                        var modeli = await AsignadorDeHorarios.VerListado(context, User);
                        return View("Index", modeli);
                    }
                    var saveData = await context.AdmoHorarios.Where(x => x.id == ins.id).FirstOrDefaultAsync();
                    saveData.fin = ins.fin;
                    saveData.Razon = ins.Razon;
                    saveData.Horarioid = ins.Horarioid;
                    saveData.Admo = cambiador;
                    saveData.Admoid = cambiador.id;
                    saveData.fechaAsignacion = DateTime.Now;


                    await context.SaveChangesAsync();
                    var model = await AsignadorDeHorarios.VerListado(context, User);
                    return View("Index", model);
                }
                ViewBag.Err = "Error, intente mas tarde.";
                var modela = await AsignadorDeHorarios.VerListado(context, User);
                return View("Index", modela);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                ViewBag.Err = "Algo no esta bien, intente mas tarde.";
                var model = await AsignadorDeHorarios.VerListado(context, User);
                return View("Index", model);
            }
        }

        /// <summary>
        /// para validar que la edicion y los datos sean correctos
        /// </summary>
        /// <param name="id"></param>
        /// <param name="idEmpleado"></param>
        /// <param name="inicio"></param>
        /// <param name="fin"></param>
        /// <returns></returns>
        private async System.Threading.Tasks.Task<bool> validarEdit(int id, int idEmpleado, DateTime inicio,DateTime fin)
        {
            if (fin <= inicio)
                return false;

            var ent = await context.AdmoHorarios.Where(x => x.inicio <= fin && x.id != id && x.Empleadoid == idEmpleado).ToListAsync();
            return ent.Count < 1;
        } 
        #endregion
    }
}
