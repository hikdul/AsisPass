using AsisPas.Helpers;
using System.ComponentModel.DataAnnotations;

namespace AsisPas.Entitys
{
    /// <summary>
    /// para implementar los lugares de trabajo de cada empresa
    /// </summary>
    public class Sedes: Iid,IAct
    {
        /// <summary>
        /// id
        /// </summary>
        [Key]
        public int id { get; set; }
        /// <summary>
        /// nombre de la sede
        /// </summary>
        [Required(ErrorMessage ="Un Nombre es obligatorio")]
        [StringLength(50)]
        public string Nombre { get; set; }

        /// <summary>
        /// direccion de donde se ubica la sede
        /// </summary>
        [Required(ErrorMessage ="La direccion es vital")]
        [StringLength(100)]
        public string Direccion { get; set; }
        /// <summary>
        /// empresa a la que pertenece la sede
        /// </summary>
        [Required(ErrorMessage ="La Sede necesita su empresa de destino")]
        public int  Empresaid { get; set; }
        /// <summary>
        /// prop nav empresa
        /// </summary>
        public Empresa Empresa { get; set; }

        // ++++++
        // esto datos estan alli por si algun dia los agregamos
        // ++++++

        /// <summary>
        /// latitud
        /// </summary>
        public double lat { get; set; }
        /// <summary>
        /// longitud
        /// </summary>
        public double lng { get; set; }
        /// <summary>
        /// si se encuentra activo o no
        /// </summary>
        public bool act { get; set; }
    }
}
