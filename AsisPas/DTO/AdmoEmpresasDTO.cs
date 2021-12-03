namespace AsisPas.DTO
{
    /// <summary>
    /// datos para el listado de admo empresas
    /// </summary>
    public class AdmoEmpresasDTO
    {
        /// <summary>
        /// user id
        /// </summary>
        public int userid { get; set; }
        /// <summary>
        /// nombre
        /// </summary>
        public string Nombre { get; set; }
        /// <summary>
        /// correo Electronico
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// numero telefonico
        /// </summary>
        public string numero { get; set; }
        /// <summary>
        /// nombre de la empresa a la que pertenece
        /// </summary>
        public string Empresa { get; set; }
    }
}
