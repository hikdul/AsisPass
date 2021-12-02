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
            SedesMap();
            HorariosMap();
        }

        #endregion

        #region empresa

        private void EmpresaMap()
        {
            CreateMap<Empresa,EmpresaDTO>().ReverseMap();
            CreateMap<empresaDTO_in, Empresa>()
                .ForMember(x => x.Logo, opt => opt.Ignore())
                .ForMember(x => x.act, opt => opt.MapFrom(y => true));
            CreateMap<Empresa, empresaDTO_in>()
               .ForMember(x => x.Logo, opt => opt.Ignore());
        }

        #endregion

        #region sedes
        private void SedesMap()
        {
            CreateMap<SedeDTO_in, Sedes>()
                .ForMember(x => x.lat, opt => opt.MapFrom(y => 0))
                .ForMember(x => x.lng, opt => opt.MapFrom(y => 0))
                .ForMember(x => x.act, opt => opt.MapFrom(y => true));
            CreateMap<Sedes, SedeDTO>()
                .ForMember(x => x.NombreEmpresa, opt => opt.MapFrom(NameEmp));
            CreateMap<SedeDTO, SedeDTO_Details>();
            CreateMap<SedeDTO_up, Sedes>()
                .ForMember(x => x.lat, opt => opt.MapFrom(y => 0))
                .ForMember(x => x.lng, opt => opt.MapFrom(y => 0))
                .ForMember(x => x.act, opt => opt.MapFrom(y => true))
                .ReverseMap();
        }
        private string NameEmp(Sedes dto, SedeDTO ent)
        {
            if (dto.Empresaid < 1)
                return "--";
            return dto.Empresa.Nombre;
        }
        #endregion

        #region Horarios

        private void HorariosMap()
        {
            CreateMap<HorarioDTO_in, Horario>().ForMember(x => x.act, y => y.MapFrom(p => true)).ReverseMap();
            CreateMap<HorarioDTO_up, Horario>().ForMember(x => x.act, y => y.MapFrom(p => true)).ReverseMap();
            CreateMap<Horario, HorarioDTO>()
                .ForMember(x => x.Empresa, opt => opt.MapFrom(NameEmp));
            CreateMap<Horario, HorarioDTO_Details>();
        }

        private string NameEmp(Horario dto, HorarioDTO ent)
        {
            if (dto.Empresaid < 1)
                return "--";
            return dto.Empresa.Nombre;
        }


        #endregion
    }
}
