using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AsisPas.Reportes
{
    /// <summary>
    /// para enviar los datos que luega se transforman en reportes
    /// </summary>
    public class Finder
    {

        #region props
        /// <summary>
        /// lista de empleados seleccionados
        /// </summary>
        [Required(ErrorMessage ="Seleccione al menos un empleado")]
        public List<int> Empleadoids { get; set; }
        /// <summary>
        /// inicio del periodo
        /// </summary>
        [Required(ErrorMessage ="Ingrese una fecha de inicio")]
        public DateTime inicio { get; set; }
        /// <summary>
        /// fin del periodo
        /// </summary>
        [Required(ErrorMessage ="Ingrese la fecha en la que acaba el periodo")]
        public DateTime fin { get; set; }

        #endregion

        #region validate
        /// <summary>
        /// valida los elementos
        /// </summary>
        /// <returns></returns>
        public bool validate()
        {
            if(this.inicio.ToString("dd/MM/yyyy") == "1/1/ 0001" || this.fin.ToString("dd/MM/yyyy") == "1/1/ 0001")
                return false;

            return inicio <= fin;
        }

        #endregion

    }
}
