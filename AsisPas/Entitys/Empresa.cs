

using AsisPas.Helpers;
using System.ComponentModel.DataAnnotations;

namespace AsisPas.Entitys
{
    /// <summary>
    /// entidad para empresas
    /// </summary>
    public class Empresa : Iid,IAct
    {
        /// <summary>
        /// key
        /// </summary>
        [Key]
        public int id { get; set; }
        /// <summary>
        /// nombre de la empresa
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Nombre { get; set; }
        /// <summary>
        /// rut de la empresa
        /// </summary>
        [Required(ErrorMessage ="el Rut es necesario")]
        [StringLength(12)]
        public string Rut { get; set; }
        /// <summary>
        /// rubro en el que participa
        /// </summary>
        [StringLength(30)]
        public string Rubro { get; set; }
        /// <summary>
        /// logo de la empresa
        /// </summary>
        public string Logo { get; set; }
        /// <summary>
        /// si la empresa se encuentra activa o no
        /// </summary>
        public bool act { get; set; }
    }
}
