using AsisPas.Data;
using AsisPas.Entitys;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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

    }
}
