using System;
using System.ComponentModel.DataAnnotations;

namespace AsisPas.Reportes
{
    /// <summary>
    /// para generar el reporte de imprecion
    /// </summary>
    public class ToPrint
    {
        #region props
        /// <summary>
        /// lista de empleados seleccionados
        /// </summary>
        [Required(ErrorMessage = "Seleccione al menos un empleado")]
        public int Empleadoid { get; set; }
        /// <summary>
        /// inicio del periodo
        /// </summary>
        [Required(ErrorMessage = "Ingrese una fecha de inicio")]
        public DateTime Finicio { get; set; }
        /// <summary>
        /// fin del periodo
        /// </summary>
        [Required(ErrorMessage = "Ingrese la fecha en la que acaba el periodo")]
        public DateTime Ffin { get; set; }

        #endregion

        #region validate
        /// <summary>
        /// valida los elementos
        /// </summary>
        /// <returns></returns>
        public bool validate()
        {
            if (this.Finicio.ToString("dd/MM/yyyy") == "1/1/ 0001" || this.Ffin.ToString("dd/MM/yyyy") == "1/1/ 0001")
                return false;

            return Finicio <= Ffin;
        }

        #endregion

    }
}
