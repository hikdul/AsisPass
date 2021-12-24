using AsisPas.Data;
using AsisPas.Helpers;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;

namespace AsisPas.Entitys
{
    /// <summary>
    /// clase para la entidad de horario
    /// </summary>
    public class Horario : Iid, IAct
    {
        #region propiedades
        /// <summary>
        /// id
        /// </summary>
        [Key]
        public int id { get; set; }

        /// <summary>
        /// nombre del horario
        /// </summary>
        [Required(ErrorMessage = "Ingrese un Nombre para el Horraio")]
        [StringLength(25)]
        public string Nombre { get; set; }
        /// <summary>
        /// inicio jornada
        /// </summary>
        [Required(ErrorMessage = "Hora de inincio jornada obligatoria")]
        [StringLength(5)]
        public string hi { get; set; }
        /// <summary>
        /// fin de jordana
        /// </summary>
        [Required(ErrorMessage = "Hora fin de jornada obligatoria")]
        [StringLength(5)]
        public string hf { get; set; }
        /// <summary>
        /// inicio break
        /// </summary>
        [StringLength(5)]
        public string hbi { get; set; }
        /// <summary>
        /// fin break
        /// </summary>
        [StringLength(5)]
        public string hbf { get; set; }
        /// <summary>
        /// indica si la jornada culmina el dia siguiente
        /// </summary>
        public bool diaSiguiente { get; set; } = false;
        /// <summary>
        /// indica si la jornada no tiene descazo
        /// </summary>
        public bool sinDescanzo { get; set; } = true;
        /// <summary>
        /// indica si el horario se encuentra activo
        /// </summary>
        public bool act { get; set; }
        /// <summary>
        /// empresa a la que pertenece el horario
        /// </summary>
        [Required(ErrorMessage = "Empresa a la que pertenece el horario")]
        public int Empresaid { get; set; }
        /// <summary>
        /// prop de navegarion
        /// </summary>
        public Empresa Empresa { get; set; }

        /// <summary>
        /// indica si aplica para el dia
        /// </summary>
        public bool Domingo { get; set; }

        /// <summary>
        /// indica si aplica para el dia
        /// </summary>
        public bool Lunes { get; set; }

        /// <summary>
        /// indica si aplica para el dia
        /// </summary>
        public bool Martes { get; set; }

        /// <summary>
        /// indica si aplica para el dia
        /// </summary>
        public bool Miercoles { get; set; }

        /// <summary>
        /// indica si aplica para el dia
        /// </summary>
        public bool Jueves { get; set; }

        /// <summary>
        /// indica si aplica para el dia
        /// </summary>
        public bool Viernes { get; set; }

        /// <summary>
        /// indica si aplica para el dia
        /// </summary>
        public bool Sabado { get; set; }


        #endregion


        #region funciones

        /// <summary>
        /// verifica si este dia es laboral o na
        /// </summary>
        /// <param name="dia"></param>
        /// <returns></returns>
        public bool DiaLaboral(int dia)
        {
            switch (dia)
            {
                case 0:
                    return Domingo;
                case 1:
                    return Lunes;
                case 2:
                    return Martes;
                case 3:
                    return Miercoles;
                case 4:
                    return Jueves;
                case 5:
                    return Viernes;
                case 6:
                    return Sabado;
                default:
                    return false;
            }
        }

        /// <summary>
        /// para obtener el tiempo laborado en segundos
        /// </summary>
        /// <returns></returns>
        public double TiempoLaborado()
        {

            DateTime hi = comvertirStraHora1990(this.hi);
            DateTime hf = comvertirStraHora1990(this.hf);

            var resp = (hi - hf).TotalMilliseconds;

            return resp - TiempoDescansado();



        }
        /// <summary>
        /// para obtener el tiempo de descanzo en segundos
        /// </summary>
        /// <returns></returns>
        public double TiempoDescansado()
        {
            if (String.IsNullOrEmpty(this.hbi) || String.IsNullOrEmpty(this.hbf))
                return 0;

            DateTime hbi = comvertirStraHora1990(this.hbi);
            DateTime hbf = comvertirStraHora1990(this.hbf);

            var resp = hbi - hbf;
            return resp.TotalSeconds;
        }


        private DateTime comvertirStraHora1990(string hora, bool otrodia = false)
        {
            var split = hora.Split(":");
            int hor = Int32.Parse(split[0]);
            int min = Int32.Parse(split[1]);
            return new(1990, 22, 3, hor, min, 3);
        }
        /// <summary>
        /// para obtener la lista de dias, segun su inicial
        /// </summary>
        /// <returns></returns>
        public string ListDiasLaborales()
        {
            string resp = "";

            if (this.Lunes)
                resp += "L ";
            if (this.Martes)
                resp += "M ";
            if (this.Miercoles)
                resp += "Mi ";
            if (this.Jueves)
                resp += "J ";
            if (this.Viernes)
                resp += "V ";
            if (this.Sabado)
                resp += "S ";
            if (this.Domingo)
                resp += "D ";

            return resp;
        }

        #endregion

        #region obtener horarios por tipo de usuario
        /// <summary>
        /// para obtener los horarios en base al tipo de usuario
        /// </summary>
        /// <param name="context"></param>
        /// <param name="User"></param>
        /// <returns></returns>
        public static async System.Threading.Tasks.Task<List<Horario>> ListadoPorUsuario(ApplicationDbContext context, ClaimsPrincipal User)
        {
            List<Horario> list = new();
            var empresas = await Empresa.FiltrarEmpresas(context, User);

            foreach (var emp in empresas)
            {
                var flag = await context.Horarios.Where(x => x.Empresaid == emp.id && x.act == true).ToListAsync();
                foreach (var h in flag)
                    list.Add(h);
            }

            return list;
        }


        /// <summary>
        /// listado por usuario completa
        /// </summary>
        /// <param name="context"></param>
        /// <param name="User"></param>
        /// <returns></returns>
        public static async System.Threading.Tasks.Task<List<Horario>> ListadoPorUsuarioComplete(ApplicationDbContext context, ClaimsPrincipal User)
        {
            List<Horario> list = new();
            var empresas = await Empresa.FiltrarEmpresas(context, User);

            foreach (var emp in empresas)
            {
                var flag = await context.Horarios.Include(x => x.Empresa).Where(x => x.Empresaid == emp.id && x.act == true).ToListAsync();
                foreach (var h in flag)
                    list.Add(h);
            }

            return list;
        }

        #endregion

        #region obtener horario por fecha y empleado
        /// <summary>
        /// para obtener un horario para una fecha y un empleado
        /// </summary>
        /// <param name="idEmpleado"></param>
        /// <param name="fecha"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static async System.Threading.Tasks.Task<Horario> HorarioPorIdEmpleado(int idEmpleado, DateTime fecha, ApplicationDbContext context)
        {
            try
            {
                string date = fecha.ToString("dd/MM/yyyy");
                var ah = await context.AdmoHorarios
                    .Include(x => x.Horario)
                    .Where(x => x.Empleadoid == idEmpleado && x.inicio <= fecha && x.fin >= fecha)
                    .FirstOrDefaultAsync();

                if (ah == null)
                    return null;
                return ah.Horario;
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex.Message);
                return null;
            }

        }
        /// <summary>
        /// para obtener un horario segun la fecha
        /// </summary>
        /// <param name="ah"></param>
        /// <param name="fecha"></param>
        /// <returns></returns>
        public static Horario HorarioPorFecha(List<AdmoHorario> ah, DateTime fecha)
        { 
            var resp =  ah.Where(x => x.inicio >= fecha && x.fin <= fecha).First();
            return resp.Horario;
        }

#endregion

        #region aux para vistas

        /// <summary>
        /// para retornar 1 elemento como select
        /// </summary>
        /// <param name="emp"></param>
        /// <param name="select"></param>
        /// <returns></returns>
        public static SelectListItem toSelect(Horario emp, bool select)
        {
            return new SelectListItem()
            {
                Text = emp.Nombre,
                Value = emp.id.ToString(),
                Selected = select
            };
        }

        /// <summary>
        /// para el retorno en lista
        /// </summary>
        /// <param name="emp"></param>
        /// <param name="idSelect"></param>
        /// <returns></returns>
        public static List<SelectListItem> toSelect(List<Horario> emp, int idSelect = 0)
        {
            List<SelectListItem> ret = new();
            foreach (var item in emp)
                ret.Add(toSelect(item, item.id == idSelect));
            return ret;
        }
        #endregion

        #region calcular tiempo entre horas del horario
        /// <summary>
        /// retorna el tiempo en segundos de un horario
        /// </summary>
        /// <param name="horario"></param>
        /// <returns></returns>
        public static double tiempoEnSegundos(Horario horario)
        {
            try
            {
                DateTime date = new DateTime(1990, 3, 22);

                var hi = ComvertirHoraAFecha(horario.hi, date);
                var hf = ComvertirHoraAFecha(horario.hf, date);
                var hbi = ComvertirHoraAFecha(horario.hbi, date);
                var hbf = ComvertirHoraAFecha(horario.hbf, date);


                return ((hi - hf) -(hbi - hbf)).TotalSeconds;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }
        }

        /// <summary>
        /// para comvertir una hora a una fecha
        /// </summary>
        /// <param name="hora"></param>
        /// <param name="fecha"></param>
        /// <returns></returns>
        public static DateTime ComvertirHoraAFecha(string hora, DateTime fecha)
        {

            int y = fecha.Year;
            int M = fecha.Month;
            int d = fecha.Day;

            var split = hora.Split(':');

            int h = split[0]== null ? 0 :Int32.Parse(split[0])  ;
            int m = split[1]== null ? 0 :Int32.Parse(split[1])  ;
            int s = split[2]== null ? 0 : Int32.Parse(split[2]);

            return new DateTime(y, M, d, h, m, s);

        }

        #endregion

    }
}
