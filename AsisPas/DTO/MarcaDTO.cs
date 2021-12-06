using AsisPas.Data.Migrations;
using AsisPas.Entitys;
using System;

namespace AsisPas.DTO
{
    /// <summary>
    /// para entregar los datos al cliente
    /// </summary>
    public class MarcaDTO
    {
        #region propiedades
        /// <summary>
        /// la marca con hora y fecha
        /// </summary>
        public DateTime marca { get; set; }
        /// <summary>
        /// prop de nav
        /// </summary>
        public string Sede { get; set; }
        /// <summary>
        /// Codigo de la puerta
        /// </summary>
        public string Gate { get; set; }
        /// <summary>
        /// nombre
        /// </summary>
        public string EmpleadoNombre { get; set; }
        /// <summary>
        /// apellido
        /// </summary>
        public string EmpleadoApellido { get; set; }
        /// <summary>
        /// rut del empleado
        /// </summary>
        public string EmpleadoRut { get; set; }
        /// <summary>
        /// horario nombre
        /// </summary>
        public string HorarioNombre { get; set; }

        /// <summary>
        /// tipo de ingreso
        /// </summary>
        public string TipoIngreso { get; set; }
        /// <summary>
        /// hash
        /// </summary>
        public string Hash { get; set; }
        /// <summary>
        /// key
        /// </summary>
        public string key { get; set; }
        #endregion

    }
}
