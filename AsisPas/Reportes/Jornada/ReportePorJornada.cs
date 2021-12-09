using AsisPas.Data;
using AsisPas.Entitys;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AsisPas.Reportes.Jornada
{
    /// <summary>
    /// para generar el reporte por jornada
    /// </summary>
    public class ReportePorJornada : CabeceraReportes
    {
        #region props
        /// <summary>
        /// sobre los dias totales de estudio
        /// </summary>
        public List<DiaJornada> Dias { get; set; }
        /// <summary>
        /// para almacenar los totales por semana
        /// </summary>
        public List<TotalesJornada> TotalesSemanales { get; set; }
        /// <summary>
        /// verifica el total del periodo
        /// </summary>
        public string TotalPeriodo { get; set; }

        #endregion


        #region llenarData
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
                // lleno mis fechas
                await UpHead(idEmpleado, inicio, fin, context);

                var ah = await context.AdmoHorarios
                    .Include(x => x.Horario)
                    .Where(x => x.Empleadoid == idEmpleado && x.fin >= inicio && x.inicio <= fin)
                    .ToListAsync();

                DateTime inicioSem = new();
                double contSem = 0;
                double contPer = 0;

                //completar el paseo para llenar el reporte... les extras incidencias, cambios y demas los agrego al final.
                for (DateTime i = inicio; i <= fin; i = i.AddDays(1))
                {
                    if (i == inicio)
                        inicioSem = i;
                    var Horario = ah.Where(x => x.inicio<= i && x.fin >= i).FirstOrDefault().Horario;
                    if(Horario != null && Horario.id > 0)
                    {
                        if (Horario.DiaLaboral(i.Day))
                        {
                            var marcas = await Marca.MarcasDiaYUsuario(idEmpleado, i, context);
                            double tiempoHoy = RTime.ObtenerTotalSg(
                                    marcas.First(x => x.TipoIngreso == 0).marca,
                                    marcas.First(x => x.TipoIngreso == 3).marca,
                                    marcas.First(x => x.TipoIngreso == 1).marca,
                                    marcas.First(x => x.TipoIngreso == 2).marca
                                    );

                            contSem += tiempoHoy;
                            contPer += tiempoHoy;

                            Dias.Add(new()
                            {
                                fecha = i,
                                horario = new()
                                {
                                    FinDescazo = Horario.hbf,
                                    FinJornada = Horario.hf,
                                    InicioDescanzo = Horario.hbi,
                                    InicioJornada = Horario.hi,
                                },
                                marcas = new()
                                {
                                    InicioJornada = marcas.First(x => x.TipoIngreso == 0).marca.ToString("HH:mm:ss"),
                                    FinJornada = marcas.First(x => x.TipoIngreso == 3).marca.ToString("HH:mm:ss"),
                                    InicioDescanzo = marcas.First(x => x.TipoIngreso == 1).marca.ToString("HH:mm:ss"),
                                    FinDescazo = marcas.First(x => x.TipoIngreso == 2).marca.ToString("HH:mm:ss")
                                },
                                tiempoLaborado = new RTime(tiempoHoy).toStr()
                            });


                            if(i.Day == 6 && i != fin)
                            {
                                var band = contSem;
                                this.TotalesSemanales.Add(new(inicioSem, i, band));
                                contSem = 0;
                                inicioSem = i.AddDays(1);
                            }
                            if(i == fin)
                            {
                                this.TotalesSemanales.Add(new(inicioSem, i, contSem));
                                RTime flag = new(contPer);
                                this.TotalPeriodo = flag.toStr();
                            }

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

