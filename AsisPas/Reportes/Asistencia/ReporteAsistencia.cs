using AsisPas.Data;
using AsisPas.Entitys;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AsisPas.Reportes
{
    /// <summary>
    /// para generar un reporte por asistencia
    /// </summary>
    public class ReporteAsistencia : CabeceraReportes
    {
        #region prop


        /// <summary>
        /// lista del recorrido por los dias habiles
        /// </summary>
        public List<DiasAsistencia> Recorrido { get; set; }
        #endregion

        #region llenar datos 
        /// <summary>
        /// para llenar los datos de mi reporte por asistencia
        /// </summary>
        /// <param name="idEmpleado"></param>
        /// <param name="inicio"></param>
        /// <param name="fin"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task Up(int idEmpleado, DateTime inicio, DateTime fin, ApplicationDbContext context)
        {
            try
            {
                // lleno mis fechas
               await UpHead(idEmpleado, inicio, fin, context);    

                AdmoHorario ah = await context.AdmoHorarios
                    .Include(x => x.Horario)
                    .FirstOrDefaultAsync(x => x.Empleadoid == idEmpleado && x.inicio >= inicio && x.fin <= fin);
                List<Feriado> feriados = await Feriado.listado(context);
                List<DiasAsistencia> dias = new();

                for (DateTime i = inicio; i <= fin; i = i.AddDays(1))
                {
                    var istr = i.ToString("dd/MM/yyyy");
                    var fer = feriados.FirstOrDefault(x => x.fecha.ToString("dd/MM/yyyy") == istr);
                    var laboral = ah.Horario.DiaLaboral((int)i.DayOfWeek);

                    if (laboral && (fer == null || fer.id < 1))
                    {
                        if (ah == null)
                            dias.Add(new(i, false, new()
                            {
                                id = -1,
                                Desc = "El Usuario no estaba contratado o no hay registros para este dia"
                            }));
                        else
                        {
                            if (ah.fin > i)
                            {
                                    ah = await context.AdmoHorarios
                                .Include(x => x.Horario)
                                .FirstOrDefaultAsync(x => x.Empleadoid == idEmpleado && x.inicio >= i && x.fin <= i);

                            }

                            var ri = await context.Marcaciones.Where(x =>
                            x.Empleadoid == idEmpleado 
                            && x.marca.ToString("dd/MM/yyyy") == istr 
                            && x.TipoIngreso == 0 )
                                .FirstOrDefaultAsync();
                            var per = await context.Permisos.Where(x =>
                            x.Empleadoid == idEmpleado
                            && x.inicio <= i && x.fin >= i).FirstOrDefaultAsync();

                            dias.Add(new(i, (ri != null || ri.id < 1), per));

                        }


                    }
                }



            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        #endregion

        #region exportar en excel
        /// <summary>
        /// para exportar un reporte en excel
        /// </summary>
        /// <param name="reporte"></param>
        /// <returns></returns>
        public static byte[] Excel(ReporteAsistencia reporte)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    using (ExcelPackage ep = new ExcelPackage())
                    {
                        int fila = 9;
                        ep.Workbook.Worksheets.Add("Reporte Por Asistencia");
                        ExcelWorksheet ew = ep.Workbook.Worksheets[0];

                        ew.Cells[1, 1].Value = "Reporte Generado el ";
                        ew.Cells[1, 2].Value = DateTime.Now.ToString("dd/MM/yyyy");

                        ew.Cells[3, 1].Value = "Trabajador";
                        ew.Cells[3, 2].Value = reporte.NombreEmpleado;
                        ew.Cells[3, 3].Value = "Rut";
                        ew.Cells[3, 4].Value = reporte.RutEmpleado;

                        ew.Cells[4, 1].Value = "Empresa";
                        ew.Cells[4, 2].Value = reporte.NombreEmpresa;
                        ew.Cells[4, 3].Value = "Rut";
                        ew.Cells[4, 4].Value = reporte.RutEmpresa;


                        ew.Cells[5, 1].Value = "Periodo de estudio";
                        ew.Cells[5, 2].Value = "Inicia";
                        ew.Cells[5, 3].Value = reporte.inicio.ToString("dd/MM/yyyy");
                        ew.Cells[5, 4].Value = "Termina";
                        ew.Cells[5, 5].Value = reporte.fin.ToString("dd/MM/yyyy");

                        ew.Cells[6, 1].Value = "Lugar de prestacion de servicio";
                        ew.Cells[6, 2].Value = reporte.NombreSede;
                        ew.Cells[6, 3].Value = reporte.DireccionDelaSede;
                        ew.Cells[6, 5].Value = "Turno";
                        ew.Cells[6, 6].Value = reporte.NombreHorario;

                        ew.Cells[8, 1].Value = "Fecha";
                        ew.Cells[8, 2].Value = "Asistio";
                        ew.Cells[8, 3].Value = "Observaciones";


                        if(reporte.Recorrido != null && reporte.Recorrido.Count > 0)
                        foreach (var item in reporte.Recorrido)
                        {
                            ew.Cells[fila, 1].Value = item.fecha;
                            ew.Cells[fila, 2].Value = item.Asistio;
                            ew.Cells[fila, 3].Value = item.Observacion;
                            if(item.permiso != null && item.permiso.id > 0 && item.permiso.act == true)
                            {
                                ew.Cells[fila, 4].Value = $"Periodo del permiso, del {item.permiso.inicio.ToString("dd/MM/yyyy")} al {item.permiso.fin.ToString("dd/MM/yyyy")} ";

                            }
                            fila++;
                        }


                        ep.SaveAs(ms);
                        return ms.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception Catch!!");
                Console.WriteLine($"Message: {0}", ex.Message);
                return new Byte[0];
            }

        }

#endregion

    }
}
