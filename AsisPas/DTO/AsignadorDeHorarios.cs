using AsisPas.Data;
using AsisPas.Entitys;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AsisPas.DTO
{
    /// <summary>
    /// para mostrar los datos del asignador de horarios
    /// </summary>
    public class AsignadorDeHorarios
    {
        #region propiedades
        // #### admo horario

        /// <summary>
        /// id administrador de horario
        /// </summary>
        public int idAdmo { get; set; }
        /// <summary>
        /// fecha en la que inicia
        /// </summary>
        public string inicio{ get; set; }
        /// <summary>
        /// fecha en la que acaba
        /// </summary>
        public string fin { get; set; }
        /// <summary>
        /// nombre del horario asignado
        /// </summary>
        public string NombreHorario { get; set; }

        // #### empleado
        /// <summary>
        /// id del empleado
        /// </summary>
        public int idEmpleado { get; set; }
        /// <summary>
        /// nombre empleado
        /// </summary>
        public string Nombre { get; set; }
        /// <summary>
        /// apellido empleado
        /// </summary>
        public string Apellido { get; set; }
        /// <summary>
        /// rut del empleado
        /// </summary>
        public string Rut { get; set; }
        /// <summary>
        /// si tiene un horario asignado para hoy
        /// </summary>
        public bool asignado { get; set; }

        #endregion

        #region ctor
        /// <summary>
        /// constructor
        /// </summary>
        public AsignadorDeHorarios(Empleado emp, AdmoHorario admo)
        {
            this.asignado = admo != null;
            this.idEmpleado = emp.id;
            this.Nombre = emp.user.Nombres;
            this.Apellido = emp.user.Apellidos;
            this.Rut = emp.user.Rut;
            if (this.asignado)
            {
                this.idAdmo = admo.id;
                this.inicio = admo.inicio.ToString("dd/MM/yyyy");
                this.fin = admo.fin.ToString("dd/MM/yyyy"); 
                this.NombreHorario = admo.Horario.Nombre;
            }
            else
            {
                this.idAdmo = 0;
                this.inicio = null;
                this.fin = null;
                this.NombreHorario = null;
            }
        }

        #endregion

        #region funcion para llenar lista
        /// <summary>
        /// para obtener una lista de elementos
        /// </summary>
        /// <param name="context"></param>
        /// <param name="User"></param>
        /// <returns></returns>
        public static async Task<List<AsignadorDeHorarios>> VerListado(ApplicationDbContext context, ClaimsPrincipal User)
        {
            List<AsignadorDeHorarios> list = new();

            var empleados = await Empleado.EmpleadosXUsuario(context, User);

            foreach (var empleado in empleados)
            {
                var admo = await context.AdmoHorarios
                    .Include(x => x.Horario)
                    .Where(x => x.Empleadoid == empleado.id && x.inicio < DateTime.Now && x.fin >= DateTime.Now)
                    .FirstOrDefaultAsync();
                if (admo == null || admo.id < 1)
                    admo = null;

                list.Add(new(empleado, admo));
            }



            return list;

        }

        #endregion

    }
}
