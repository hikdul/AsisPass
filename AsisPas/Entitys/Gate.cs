using AsisPas.Helpers;
using System.ComponentModel.DataAnnotations;

namespace AsisPas.Entitys
{
    /// <summary>
    /// asignacion de puntos de acceso
    /// </summary>
    public class Gate: MD5, Iid,IAct
    {
        #region prop

        /// <summary>
        /// id
        /// </summary>
        [Key]
        public int id { get; set; }
        /// <summary>
        /// activo
        /// </summary>
        public bool act { get; set; } = true;
        /// <summary>
        /// codigo de identificacion
        /// </summary>
        [Required]
        public string code { get; set; }
        /// <summary>
        /// sede a la que pertenece
        /// </summary>
        [Required]
        public int Sedeid { get; set; }
        /// <summary>
        /// prop de navegacion
        /// </summary>
        public Sedes Sede { get; set; }
        /// <summary>
        /// para agregar una descripcion de la gate
        /// </summary>
        [StringLength(20)]
        public string Desc { get; set; } = null;

        #endregion

        #region complete
        /// <summary>
        /// para generar el code
        /// </summary>
        public void complete()
        {
            this.code = Encriptar($"sede:{this.Sedeid}||{this.Desc}");
        }

        #endregion
    }
}
