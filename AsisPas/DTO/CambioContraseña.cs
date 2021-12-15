using System.ComponentModel.DataAnnotations;

namespace AsisPas.DTO
{
    /// <summary>
    /// para enviar los datos para cambiar la contraseña
    /// </summary>
    public class CambioContraseña
    {
        /// <summary>
        /// psw actual
        /// </summary>
        [Required(ErrorMessage ="Debe ingresar algo aca")]
[Display(Name ="Contraseña Actual")]
        public string Anterior { get; set; }
        /// <summary>
        /// nueva
        /// </summary>
        [Required(ErrorMessage ="Debe ingresar algo aca")]
        [Display (Name ="Contraseña Nueva")]
        public string nueva { get; set; }
        /// <summary>
        /// verificar nueva
        /// </summary>
        [Required(ErrorMessage = "Debe ingresar algo aca")]
        [Display (Name ="Repita contraseña")]
        public string nueva2 { get; set; }

        /// <summary>
        /// para validar
        /// </summary>
        /// <returns></returns>
        public bool validate()
        {
           return this.Anterior != this.nueva && this.nueva == this.nueva2;
        }

    }
}
