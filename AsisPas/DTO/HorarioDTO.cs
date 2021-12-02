using System.ComponentModel.DataAnnotations;

namespace AsisPas.DTO
{
    /// <summary>
    /// para ver los datos del horaio
    /// </summary>
    public class HorarioDTO
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
        public string Nombre { get; set; }
        /// <summary>
        /// inicio jornada
        /// </summary>
        public string Empresa { get; set; }
      

        #endregion

    }
}
