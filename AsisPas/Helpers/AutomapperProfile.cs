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
            UsuariosMap();
            AdmoHorarioMap();
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

            CreateMap<Horario, Horario>();
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

        #region usuarios

        private void UsuariosMap()
        {
            CreateMap<AdmoEmpresaDTO_in, Usuario>()
                .ForMember(x => x.Hash, opt => opt.Ignore())
                .ForMember(x => x.Salt, opt => opt.Ignore())
                .ForMember(x => x.user, opt => opt.Ignore())
                .ForMember(x => x.userid, opt => opt.Ignore())
                .ForMember(x => x.id, opt => opt.Ignore());

            CreateMap<AdmoEmpresas, AdmoEmpresasDTO>()
                .ForMember(x => x.Empresa, opt => opt.MapFrom(NameEmp))
                .ForMember(x => x.Email, opt => opt.MapFrom(EmailTo))
                .ForMember(x => x.Nombre, opt => opt.MapFrom(NameTo))
                .ForMember(x => x.numero, opt => opt.MapFrom(NumberTo));

            CreateMap<UsuarioDTO_in,Usuario>()
                 .ForMember(x => x.Hash, opt => opt.Ignore())
                .ForMember(x => x.Salt, opt => opt.Ignore())
                .ForMember(x => x.user, opt => opt.Ignore())
                .ForMember(x => x.userid, opt => opt.Ignore())
                .ForMember(x => x.id, opt => opt.Ignore());



        }
        /// <summary>
        /// email
        /// </summary>
        /// <param name="In"></param>
        /// <param name="Out"></param>
        /// <returns></returns>
        private string EmailTo(AdmoEmpresas In, AdmoEmpresasDTO Out)
        {
            if (In.userid < 1)
                return "--";
            return In.user.Email;
        }
        /// <summary>
        /// nombre
        /// </summary>
        /// <param name="In"></param>
        /// <param name="Out"></param>
        /// <returns></returns>
        private string NameTo(AdmoEmpresas In, AdmoEmpresasDTO Out)
        {
            if (In.userid < 1)
                return "--";
            return In.user.Nombres;
        }
        /// <summary>
        /// numero
        /// </summary>
        /// <param name="In"></param>
        /// <param name="Out"></param>
        /// <returns></returns>
        private string NumberTo(AdmoEmpresas In, AdmoEmpresasDTO Out)
        {
            if (In.userid < 1)
                return "--";
            return In.user.telefono == null ? " -- " : In.user.telefono;
        }
        /// <summary>
        /// para obtener el nobre de la empresa
        /// </summary>
        /// <param name="In"></param>
        /// <param name="Out"></param>
        /// <returns></returns>
        private string NameEmp(AdmoEmpresas In, AdmoEmpresasDTO Out)
            {
                if (In.Empresaid < 1)
                    return "--";
                return In.Empresa.Nombre;
            }

        #endregion

        #region Administrador de Horario

        private void AdmoHorarioMap()
        {
            CreateMap<AdmoHorarioDTO_in, AdmoHorario>()
            .ForMember(x => x.Admo, opt => opt.Ignore())
            .ForMember(x => x.Admoid, opt => opt.Ignore())
            .ForMember(x => x.Empleado, opt => opt.Ignore())
            .ForMember(x => x.Horario, opt => opt.Ignore());

            CreateMap<AdmoHorario,AdmoHorarioDTO_in>();

            CreateMap<AdmoHorarioDTO_up, AdmoHorario>()
           .ForMember(x => x.Admo, opt => opt.Ignore())
           .ForMember(x => x.Admoid, opt => opt.Ignore())
           .ForMember(x => x.Empleado, opt => opt.Ignore())
           .ForMember(x => x.Horario, opt => opt.Ignore());

            CreateMap<AdmoHorario, AdmoHorarioDTO_up>();


            
        }

        #endregion

        #region Gate

        private void GateMap()
        {
            CreateMap<GateDTO_in, Gate>()
                .ForMember(x => x.act, opt => opt.MapFrom(y => true))
                .ForMember(x => x.Sede, opt => opt.Ignore())
                .ForMember(x => x.code, opt => opt.Ignore());
           
        }

        #endregion
    }
}
