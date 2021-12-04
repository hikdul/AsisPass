using System.ComponentModel.DataAnnotations;

namespace AsisPas.Entitys
{
    /// <summary>
    /// para contener los datos de los fiscales
    /// </summary>
    public class Fiscal
    {
        /// <summary>
        /// id del fiscal
        /// </summary>
        [Key]
        public int id { get; set; }
        /// <summary>
        /// usuario que tiene el ros
        /// </summary>
        public int userid { get; set; }
        /// <summary>
        /// prop de nav
        /// </summary>
        public Usuario user { get; set; }
        /// <summary>
        /// empresa en la que esta a cargo
        /// </summary>
        public int Empresaid { get; set; }
        /// <summary>
        /// prop de nav
        /// </summary>
        public Empresa Empresa { get; set; }
        /// <summary>
        /// si se encuentra activo o no
        /// </summary>
        public bool act { get; set; }
    }
}
