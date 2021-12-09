namespace AsisPas.Reportes.CambiosTurno
{
    /// <summary>
    /// representa la unidad de cambio
    /// </summary>
    public class DiaCambio
    {
        /// <summary>
        /// datos sobre el ah anterior
        /// </summary>
        public modificaciones Anterior { get; set; }
        /// <summary>
        /// daton sobre el oh actual
        /// </summary>
        public modificaciones nuevo { get; set; }
        /// <summary>
        /// solicitante del cambio
        /// </summary>
        public string SolicitanteCambio { get; set; }
        /// <summary>
        /// decripcion del cambio
        /// </summary>
        public string Desc { get; set; }
    }
}
