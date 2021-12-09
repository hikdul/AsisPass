using AsisPas.Data;
using AsisPas.Entitys;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AsisPas.Reportes.Domingos
{
    /// <summary>
    /// para generar el reporte para domingos y feriados
    /// obteniendo solo el id de empleado y el rango
    /// </summary>
    public class ReporteDomingosYFeriados : CabeceraReportes
    {

        #region prop

        /// <summary>
        /// total domingos del periodo
        /// </summary>
        public int TotalDomingosPeriodo { get; set; }
        /// <summary>
        /// laborados
        /// </summary>
        public int TotalDomingos { get; set; }
        /// <summary>
        /// total feriados del periodo
        /// </summary>
        public int TotalFeriadosPeriodo { get; set; }
        /// <summary>
        /// laborados
        /// </summary>
        public int TotalFeriadosLaborados { get; set; }

        /// <summary>
        /// calculos totales por ano
        /// </summary>
        public List<RangoTotalesDomingo> totalesPorAno{ get; set; }

        /// <summary>
        /// lleva el registro mes a mes
        /// </summary>
        public List<MensualDomingos> RegitroMensual { get; set; }


        #endregion


        #region llenado de los datos
        /// <summary>
        /// para llenar los datos del reporte
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
                // ### LLENO LA CABECERA
                await UpHead(idEmpleado, inicio, fin, context);

                // ## COMPLETO EL ROPORTE
                int periodoD = 0;
                int laboradosD = 0;
                int periodoF = 0;
                int laboradosF = 0;
                int anoD = 0;
                int anoF = 0;
                int mesD = 0;
                int mesF = 0;
                int totalanoD = 0;
                int totalanoF = 0;
                int totalmesD = 0;
                int totalmesF = 0;

                DateTime anoI = new DateTime();
                DateTime mesI = new DateTime();
                int mesActual = 0;
                int anoActual = 0;
                List<DiaDomingos> diario = new();


                var feriados = await Feriado.listado(context);

                for (var i = inicio; i <= fin; i = i.AddDays(1))
                {
                    //obtengo horario
                    Horario horario = await Horario.HorarioPorIdEmpleado(idEmpleado, i, context);
                    //almacen y cambio de fechos en interaccion
                    if(i == inicio)
                    {
                        anoI = i;
                        mesI = i;
                        mesActual = i.Month;
                        anoActual = i.Year;
                    }

                    if(mesActual != i.Month)
                    {
                        var inincio = mesI;
                        var finalizo = i.AddDays(-1);
                        int mes = mesD;
                        int mess = mesF;
                        int Tmes = totalmesD;
                        int Tmess = totalmesF;
                        List<DiaDomingos> dias = diario;
                        this.RegitroMensual.Add(new()
                        {
                            Diario = dias,
                            inicio = inincio,
                            fin = finalizo,
                            totalDomingos = mes,
                            totalFeriados = mess,
                            totalDomingosenPeriodo = Tmes,
                            totalFeriadosenPeriodo = Tmess,
                        });
                        mesActual = i.Month;
                        mesD = 0;
                        mesF = 0;
                        totalmesD = 0;
                        totalmesF = 0;
                    }

                    if (anoActual != i.Year)
                    {
                        var inincio = anoI;
                        var finalizo = i.AddDays(-1);
                        int ano = anoD;
                        int anoo = anoF;
                        int Tano = totalanoD;
                        int Tanoo = totalanoF;
                        this.totalesPorAno.Add(new() { 
                            inicio = inincio,
                            fin = finalizo,
                            totalDomingos = ano,
                            totalFeriados = anoo,
                            totalDomingosenPeriodo=Tano,
                            totalFeriadosenPeriodo=Tanoo
                        });

                        anoActual = i.Year;
                        anoD = 0;
                        anoF = 0;
                        totalanoD = 0;
                        totalanoF = 0;
                    }

                    //Calculo y sumas para Domingos

                    if ((int) i.Day == 0)
                    {
                        var fecha = i.ToString("dd/MM/yyyy");
                        var mi = await context.Marcaciones.FirstOrDefaultAsync(x => x.marca.ToString("dd/MM/yyyy") == fecha && x.Empleadoid == idEmpleado);
                        var laboral = horario.DiaLaboral(i.Day);
                        laboradosD++;
                        if(mi != null)
                        {
                            periodoD++;
                            anoD++;
                            mesD++;
                            diario.Add(new()
                            {
                                Asistio = "SI",
                                fecha = i,
                                Observacion = !laboral ? "Dia De Descansa" : "",
                            });
                        }

                        if(mi == null && laboral)
                        {
                            var permiso = await context.Permisos
                                .Where(x => x.Empleadoid == idEmpleado && x.inicio <= i && x.fin >= i)
                                .FirstOrDefaultAsync();
                            diario.Add(new()
                            {
                                Asistio = "NO",
                                fecha = i,
                                Observacion = permiso == null || permiso.id < 1 ? "No Hay Justificacion Para La Falta" : permiso.Desc
                            });
                        }

                        if (mi == null && !laboral)
                        {
                            diario.Add(new()
                            {
                                Asistio = "NO",
                                fecha = i,
                                Observacion = "Dia de Descaso"
                            });
                        }

                    }

                    //Calculo y sumas para Feriados
                    var fechaFeriado = i.ToString("dd/MM");
                    if(feriados.Find(x => x.fecha.ToString("dd/MM") == fechaFeriado) != null)
                    {
                        laboradosF++;
                        var laboral = horario.DiaLaboral(i.Day);
                        var mi = await context.Marcaciones
                            .FirstOrDefaultAsync(x => 
                            x.marca.ToString("dd/MM/yyyy") == i.ToString("dd/MM/yyyy") 
                            && x.Empleadoid == idEmpleado);

                        if (mi != null)
                        {

                            periodoF++;
                            anoF++;
                            mesF++;
                            diario.Add(new()
                            {
                                Asistio = "SI",
                                fecha = i,
                                Observacion = "",
                            });
                        }

                        if (mi == null && laboral)
                        {
                            var permiso = await context.Permisos
                                .Where(x => x.Empleadoid == idEmpleado && x.inicio <= i && x.fin >= i)
                                .FirstOrDefaultAsync();
                            diario.Add(new()
                            {
                                Asistio = "NO",
                                fecha = i,
                                Observacion = permiso == null || permiso.id < 1 ? "Descanzo Por Feriado" : permiso.Desc
                            });
                        }


                        if (mi == null && !laboral)
                        {
                            diario.Add(new()
                            {
                                Asistio = "NO",
                                fecha = i,
                                Observacion = "Descanzo Por Feriado, dia no laboral"
                            });
                        }

                    }

                    // finaliza el periodo -- creo que hay que agregar el cierre de mes y de ano
                    if(i == fin)
                    {
                        //total periodo
                        this.TotalDomingosPeriodo = periodoD;
                        this.TotalFeriadosPeriodo = periodoF;
                        this.TotalDomingos = laboradosD;
                        this.TotalFeriadosLaborados = laboradosF;
                        //total mes
                        this.RegitroMensual.Add(new()
                        {
                            Diario = diario,
                            inicio = mesI,
                            fin = i,
                            totalDomingos = mesD,
                            totalFeriados = mesF,
                            totalDomingosenPeriodo = totalmesD,
                            totalFeriadosenPeriodo = totalmesF,
                        });
                        //total ano
                        this.totalesPorAno.Add(new()
                        {
                            inicio = anoI,
                            fin = i,
                            totalDomingos = anoD,
                            totalFeriados = anoF,
                            totalDomingosenPeriodo = totalanoD,
                            totalFeriadosenPeriodo = totalanoF
                        });

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
