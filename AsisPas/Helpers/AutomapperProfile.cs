using AsisPas.DTO;
using AsisPas.Entitys;
using AutoMapper;

namespace AsisPas.Helpers
{
    /// <summary>
    /// para encapsular mis mapeos
    /// </summary>
    public class AutomapperProfile: Profile
    {
#region constructor
        /// <summary>
        /// constructor
        /// </summary>
        public AutomapperProfile()
        {
            EmpresaMap();
        
        }

        #endregion

        #region empresa

        private void EmpresaMap()
        {
            CreateMap<Empresa,EmpresaDTO>().ReverseMap();
            CreateMap<empresaDTO_in, Empresa>()
                .ForMember(x => x.Logo, opt => opt.Ignore())
                .ForMember(x => x.act, opt => opt.MapFrom(y => true));
        }

        #endregion

    }
}
