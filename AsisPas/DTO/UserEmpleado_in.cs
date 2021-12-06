namespace AsisPas.DTO
{
    /// <summary>
    /// para ingresar un empleado
    /// </summary>
    public class UserEmpleado_in: AdmoEmpresaDTO_in
    {
        /// <summary>
        /// para indicar si tiene o no el derecho a este articulo
        /// </summary>
        public bool Articulo22 { get; set; } = false;
    }
}
