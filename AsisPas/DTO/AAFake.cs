using System;

namespace AsisPas.DTO
{
    /// <summary>
    /// clase para ingresar datos falsos
    /// ingresa admo horarios, marcas en periodos y vacaciones cada 12 meses
    /// </summary>
    public class AAFake
    {
        /// <summary>
        /// id del empleado
        /// </summary>
        public int idEmpleado { get; set; }
        /// <summary>
        /// id del horario
        /// </summary>
        public int idHorario { get; set; }
        /// <summary>
        /// id del admo a cargo
        /// </summary>
        public int idAdmoEmpresa { get; set; }
        /// <summary>
        /// id de la sede
        /// </summary>
        public int idSede { get; set; }
        /// <summary>
        /// desde que inicia
        /// </summary>
        public DateTime inicio{ get; set; }
        /// <summary>
        /// cuando termina
        /// </summary>
        public DateTime fin { get; set; }
        /// <summary>
        /// razon por la que se agrega el nuevo horario
        /// </summary>
        public string razon { get; set; }
    }
}
