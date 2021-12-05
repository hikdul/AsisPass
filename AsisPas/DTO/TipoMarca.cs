namespace AsisPas.DTO
{
        /// <summary>
        /// sub clase que no llevara tabla
        /// </summary>
       
    public class TipoMarca
    {
        /// <summary>
        /// valor ent
        /// 0 => inincio a jornada
        /// 1 => inicio descanso
        /// 2 => fin descanso
        /// 3 => fin de jornada
        /// </summary>
        public int value { get; set; }
        /// <summary>
        ///  desc segun su marca
        /// 0 => inincio a jornada
        /// 1 => inicio descanso
        /// 2 => fin descanso
        /// 3 => fin de jornada
        /// </summary>
        public string desc { get; set; }
        
    }
}
