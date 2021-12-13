using AsisPas.Data;
using AsisPas.Entitys;
using AsisPas.Reportes;
using AsisPas.Reportes.Domingos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;

namespace AsisPas.Controllers
{
    /// <summary>
    /// reporte de domingos y feriados
    /// </summary>
    public class ReporteDomigosYFeriadosController : Controller
    {
        #region ctor
        private readonly ApplicationDbContext context;
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="context"></param>
        public ReporteDomigosYFeriadosController(ApplicationDbContext context)
        {
            this.context = context;
        }

        #endregion


        #region pagina inicia, buscador
        /// <summary>
        /// pagina inicial
        /// </summary>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<IActionResult> Index()
        {
            try
            {
                List<ElementosBusqueda> elementos = new();
                SelectListItem empty = new()
                {
                    Value = "-1",
                    Selected = true,
                    Text = "-- ### Todo- ### --"
                };

                var turnos = Horario.toSelect(await Horario.ListadoPorUsuario(context, User));
                var faenas = Sedes.toSelect(await Sedes.ListadoPorUsuario(context, User));
                turnos.Add(empty);
                faenas.Add(empty);

                ViewBag.Turnos = turnos;
                ViewBag.Faenas = faenas;

                var empleados = await Empleado.EmpleadosXUsuarioLigth(context, User);
                foreach (var item in empleados)
                {
                    var elemento = new ElementosBusqueda();
                    await elemento.Up(item.id, context);
                    elementos.Add(elemento);
                }

                ViewBag.Empleados = elementos;

                return View();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ViewBag.Err = "Hubo un problema al cargar los datos, Por Favor intente mas tarde.";
                return View();
            }

        }



        #endregion


        #region generar reporte
        /// <summary>
        /// para generar el listado de reportes
        /// </summary>
        /// <param name="ins"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<IActionResult> Generar(Finder ins)
        {
            try
            {

                if (!ins.validate())
                    return await CargarIndex("Las fecha no son validas");
                if (ins == null || ins.Empleadoids == null || ins.Empleadoids.Count < 1)
                    return await CargarIndex("Seleccione al menos un Empleado");

                ViewBag.fechai = ins.inicio.ToString("yyyy-MM-dd");
                ViewBag.fechaf = ins.fin.ToString("yyyy-MM-dd");

                List<ReporteDomingosYFeriados> list = new();
                foreach (var id in ins.Empleadoids)
                {
                    ReporteDomingosYFeriados flag = new();
                    await flag.Up(id, ins.inicio, ins.fin, context);
                    list.Add(flag);
                }
                ViewBag.Reportes = list;
                return View();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return View();
            }
        }

        #region cargar index


        private async System.Threading.Tasks.Task<IActionResult> CargarIndex(string err = "")
        {
            if (err != "")
                ViewBag.Err = err;
            List<ElementosBusqueda> elementos = new();
            SelectListItem empty = new()
            {
                Value = "-1",
                Selected = true,
                Text = "-- ### Todo- ### --"
            };

            var turnos = Horario.toSelect(await Horario.ListadoPorUsuario(context, User));
            var faenas = Sedes.toSelect(await Sedes.ListadoPorUsuario(context, User));
            turnos.Add(empty);
            faenas.Add(empty);

            ViewBag.Turnos = turnos;
            ViewBag.Faenas = faenas;

            var empleados = await Empleado.EmpleadosXUsuarioLigth(context, User);
            foreach (var item in empleados)
            {
                var elemento = new ElementosBusqueda();
                await elemento.Up(item.id, context);
                elementos.Add(elemento);
            }

            ViewBag.Empleados = elementos;

            return View("Index");
        }

        #endregion

        #endregion

        #region Excel

        /// <summary>
        /// para generar el reporte en excel
        /// </summary>
        /// <param name="ins"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<FileResult> Excel(ToPrint ins)
        {
            try
            {
                if (!ins.validate())
                    return File(new byte[0], "application/vnd.ms-excel", "FechasNoValidas.xlsx");
                if (ins == null || ins.Empleadoid < 1)
                    return File(new byte[0], "application/vnd.ms-excel", "AlMenosSeleccioneUnEmpleado.xlsx");

                ReporteDomingosYFeriados resp = new();
                await resp.Up(ins.Empleadoid, ins.Finicio, ins.Ffin, context);
                var buffer = ReporteDomingosYFeriados.Excel(resp);
                return File(buffer, "pplication/vnd.ms-excel", "Reporte Domingos Y Feriados" + "-" + ins.Finicio + "-al-" + ins.Ffin + ".xlsx");

            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception Catch!!");
                Console.WriteLine("Exception msn: {0}", ex.Message);
                return File(new byte[0], "application/vnd.ms-excel", "Empty.xlsx");

            }
        }


        #endregion

        #region pdf
        /// <summary>
        /// para exportar en pdf
        /// </summary>
        /// <param name="ins"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<FileResult> Pdf(ToPrint ins)
        {
            try
            {
                ReporteDomingosYFeriados resp = new();
                await resp.Up(ins.Empleadoid, ins.Finicio, ins.Ffin, context);


                var buffer = ReporteDomingosYFeriados.Pdf(resp);
                return File(buffer, "application/pdf", "Reporte Domingos y feriados" + "-" + ins.Finicio.ToString("dd/MM/yyyy") + "-al-" + ins.Ffin.ToString("dd/MM/yyyy") + ".PDF");

            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception Catch!!");
                Console.WriteLine("Exception msn: {0}", ex.Message);
                return File(new byte[0], "application/pdf", "Error.PDF");

            }
        }


        #endregion

        #region word
        /// <summary>
        /// para exportar en pdf
        /// </summary>
        /// <param name="ins"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<FileResult> Word(ToPrint ins)
        {
            try
            {
                ReporteDomingosYFeriados resp = new();
                await resp.Up(ins.Empleadoid, ins.Finicio, ins.Ffin, context);


                var buffer = ReporteDomingosYFeriados.Word(resp);
                return File(buffer, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "REPORTE POR DOMINGOS Y FERIADOS -" + ins.Finicio.ToString("dd/MM/yyyy") + " al " + ins.Ffin.ToString("dd/MM/yyyy") + ".Docx");


            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception Catch!!");
                Console.WriteLine("Exception msn: {0}", ex.Message);
                return File(new byte[0], "application/pdf", "Error.PDF");

            }
        }


        #endregion

    }
}
