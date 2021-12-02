namespace AsisPas.DTO
{
    /// <summary>
    /// para ver los datos de la empresas
    /// </summary>
    public class EmpresaDTO
    {
        /// <summary>
        /// key
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// nombre de la empresa
        /// </summary>
        public string Nombre { get; set; }
        /// <summary>
        /// rut de la empresa
        /// </summary>
        public string Rut { get; set; }
        /// <summary>
        /// rubro en el que participa
        /// </summary>
        public string Rubro { get; set; }
        /// <summary>
        /// logo de la empresa
        /// </summary>
        public string Logo { get; set; }
        /// <summary>
        /// si se encuentra activa o no
        /// </summary>
        public bool act { get; set; }
    }
}
