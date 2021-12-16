using AsisPas.Data;
using AsisPas.DTO;
using AsisPas.Entitys;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace AsisPas.Controllers.API
{
    /// <summary>
    /// controlador para generar datos falsos
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class PruebasController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="context"></param>
        public PruebasController(ApplicationDbContext context)
        {
            this.context = context;
        }
        /// <summary>
        /// funcion para generar los datos falsos... solo se ingresa como SA
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async System.Threading.Tasks.Task<bool> Fake(AAFake data)
        {
            try {

                int contmeses = 0;
                int mesInicial = data.inicio.Month;
                //creo el admo de horarios
                AdmoHorario ah = new() { 
                Admoid = data.idAdmoEmpresa,
                Empleadoid = data.idEmpleado,
                fechaAsignacion = data.inicio,
                fin = data.fin,
                inicio = data.inicio,
                Horarioid = data.idHorario,
                Razon = data.razon
                };

               context.Add(ah);
                await context.SaveChangesAsync();

                var horario = await context.Horarios.Where(x => x.id == data.idHorario).FirstOrDefaultAsync();
                ///creo todos los registros por dia
                for (DateTime i = data.inicio; i <= data.fin; i = i.AddDays(1))
                {
                    
                    if(mesInicial != i.Month)
                    {
                        contmeses++;
                        mesInicial = i.Month;
                    }


                    if (horario.DiaLaboral((int)i.DayOfWeek))
                    {
                        Marca MarcaInicial = new()
                        {
                            Empleadoid = data.idEmpleado,
                            Horarioid = data.idHorario,
                            Sedeid = data.idSede,
                            TipoIngreso = 0,
                            marca = CreateMark(i, horario.hi),
                        };
                        MarcaInicial.Complete();

                        Marca MarcaIniciaDescanso = new()
                        {
                            Empleadoid = data.idEmpleado,
                            Horarioid = data.idHorario,
                            Sedeid = data.idSede,
                            TipoIngreso = 1,
                            marca = CreateMark(i, horario.hbi),
                        };
                        MarcaIniciaDescanso.Complete();

                        Marca MarcaFinDescanso = new()
                        {
                            Empleadoid = data.idEmpleado,
                            Horarioid = data.idHorario,
                            Sedeid = data.idSede,
                            TipoIngreso = 2,
                            marca = CreateMark(i, horario.hbf),
                        };
                        MarcaFinDescanso.Complete();

                        Marca MarcaFin = new()
                        {
                            Empleadoid = data.idEmpleado,
                            Horarioid = data.idHorario,
                            Sedeid = data.idSede,
                            TipoIngreso = 3,
                            marca = CreateMark(i, horario.hf),
                        };
                        MarcaFin.Complete();

                        context.Add(MarcaInicial);
                        context.Add(MarcaIniciaDescanso);
                        context.Add(MarcaFinDescanso);
                        context.Add(MarcaFin);
                    }

                    if (contmeses == 12)
                    {
                        contmeses = 0;
                        DateTime final = i.AddDays(15);
                        Permisos p = new()
                        {
                            act = true,
                            AprobadoPorid = data.idAdmoEmpresa,
                            Desc = "Vacaciones Anuales",
                            Empleadoid = data.idEmpleado,
                            inicio = i,
                            fin = final,
                        };
                        i = final;
                        context.Add(p);

                    }

                }
                await context.SaveChangesAsync();


                return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;

            }
        }


        private DateTime CreateMark(DateTime fecha, string horario)
        {
            Random rnd = new Random();
            int y = fecha.Year;
            int m = fecha.Month;
            int d = fecha.Day;

            var horas = horario.Split(":");

            int hora = Int32.Parse(horas[0]);
            var bandera = Int32.Parse(horas[1]);
            int minuto = rnd.Next(bandera  , bandera + 20);
            int segundo = rnd.Next(0 ,50);

            return new DateTime(y,m,d,hora,minuto,segundo);
        }

    }
}
