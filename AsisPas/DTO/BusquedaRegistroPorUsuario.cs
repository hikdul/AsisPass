using System;

namespace AsisPas.DTO
{
    /// <summary>
    /// para hacer la busqueda de registros
    /// se obtiene el periodo y el empledo destino
    /// </summary>
    public class BusquedaRegistroPorUsuario
    {
        /// <summary>
        /// empleado destino
        /// </summary>
        public int Empleadoid { get; set; }
        /// <summary>
        /// inicio del periodo de busqueda
        /// </summary>
        public DateTime inicio { get; set; }
        /// <summary>
        /// fin del periodo de busquedo
        /// </summary>
        public DateTime fin { get; set; }
    }
}
