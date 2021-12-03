using AsisPas.Helpers;

namespace AsisPas.Entitys
{
    /// <summary>
    /// Administradores de empresas
    /// </summary>
    public class AdmoEmpresas: IAct
    {
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
