
using AsisPas.Data;
using AsisPas.Entitys;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AsisPas.Reportes.Excesojornada
{
    /// <summary>
    /// para generar el reporte por exceso de jornada
    /// </summary>
    public class ReporteExcesoJornada: CabeceraReportes
    {
        #region props
        /// <summary>
        /// listado de dias
        /// </summary>
        public List<DiaExcesoJornada> Dias{ get; set; }
        /// <summary>
        /// listado de totales por semana
        /// </summary>
        public List<TotalesExcesoJornada> Semanal { get; set; }
        /// <summary>
        /// tiempo de exceso
        /// </summary>
        public string tiempoExceso { get; set; }
        /// <summary>
        /// tiempo de retrazo
        /// </summary>
        public string tiempoRetrazo { get; set; }
        /// <summary>
        /// Compensacion
        /// </summary>
        public string Compensacion { get; set; }
        #endregion


        #region llenar datos
        /// <summary>
        /// para llenar datos
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
                // lleno mi cabecera
                await UpHead(idEmpleado, inicio, fin, context);


                for (DateTime i = inicio; i <= fin; i = i.AddDays(1))
                {
                    var istr = i.ToString("dd/MM/yyyy");
                    var marcas = await context.Marcaciones
                        .Include(x => x.Horario)
                        .Where(x => x.Empleadoid == idEmpleado && x.marca.ToString("dd/MM/yyyy") == istr)
                        .ToListAsync();
                    foreach (var marca in marcas)
                    {
                        if (marca.Horario.DiaLaboral(i.Day))
                        {

                        }
                        else
                        {

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


        #region calculadora
        /// <summary>
        /// para calcular la diferencia entre una hora dada y su marca
        /// </summary>
        /// <param name="reg"></param>
        /// <param name="hora"></param>
        /// <returns></returns>
        private double calcularDifEnSegundos(DateTime reg, string hora)
        {
            if (string.IsNullOrEmpty(hora))
                return 0;
            var splt = hora.Split(':');
            if(splt.Length < 2)
                return 0;

            int y = reg.Year;
            int m = reg.Month;
            int d = reg.Day;
            int segundos = splt[2] == null ? 0 : Int32.Parse(splt[2]);

            DateTime horario = new DateTime(y, m, d, Int32.Parse(splt[0]), Int32.Parse(splt[1]), segundos);

            return reg.Subtract(horario).Seconds;
        }


        private double CalcularDiferencial(DateTime hi,DateTime hf, DateTime bi, DateTime bf, Horario horario)
        {
            var Ingreso = calcularDifEnSegundos(hi, horario.hi);
            var Salida = calcularDifEnSegundos(hf, horario.hf);
            var ingresoD = calcularDifEnSegundos(bi, horario.hbi);
            var SalidoD = calcularDifEnSegundos(bf, horario.hbf);

            return Ingreso + Salida + SalidoD + ingresoD;
        }

        #endregion


    }
}
