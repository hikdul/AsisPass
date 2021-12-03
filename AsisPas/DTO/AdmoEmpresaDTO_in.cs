using System.ComponentModel.DataAnnotations;

namespace AsisPas.DTO
{
    /// <summary>
    /// para agregar un nueva admo de empresas
    /// </summary>
    public class AdmoEmpresaDTO_in : UsuarioDTO_in
    {
        /// <summary>
        /// empresa a la que pertenece
        /// </summary>
        [Required]
        [Display(Name ="Seleccione Empresa")]
        public int Empresaid { get; set; }
       

    }
}
