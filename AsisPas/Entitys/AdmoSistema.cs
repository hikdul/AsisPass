using AsisPas.Helpers;

namespace AsisPas.Entitys
{
    /// <summary>
    /// para almacenar mis administradores de sistema
    /// </summary>
    public class AdmoSistema : Iid, IAct
    {
        #region propiedades
        /// <summary>
        /// id
        /// </summary>
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
        /// active
        /// </summary>
        public bool act { get; set; }
        #endregion
    }
}
