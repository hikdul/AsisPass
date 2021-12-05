using System.ComponentModel.DataAnnotations;

namespace AsisPas.DTO
{
    /// <summary>
    /// para crear una nueva gate
    /// </summary>
    public class GateDTO_in
    {
        /// <summary>
        /// sede que se desea crear el gate
        /// </summary>
        [Required]
        public int Sedeid { get; set; }
        /// <summary>
        /// descripcion
        /// </summary>
        public string Desc{ get; set; }
    }
}
