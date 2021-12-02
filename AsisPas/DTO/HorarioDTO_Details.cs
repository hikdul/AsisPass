
namespace AsisPas.DTO
{
    /// <summary>
    /// para ver todos los datos de un horario
    /// </summary>
    public class HorarioDTO_Details
    {
        #region propiedades
        /// <summary>
        /// id
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// nombre del horario
        /// </summary>
        public string Nombre { get; set; }
        /// <summary>
        /// inicio jornada
        /// </summary>
        public string hi { get; set; }
        /// <summary>
        /// fin de jordana
        /// </summary>
        public string hf { get; set; }
        /// <summary>
        /// inicio break
        /// </summary>
        public string hbi { get; set; }
        /// <summary>
        /// fin break
        /// </summary>
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
        public int Empresaid { get; set; }
        /// <summary>
        /// prop de navegarion
        /// </summary>
        public EmpresaDTO Empresa { get; set; }

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


    }
}
