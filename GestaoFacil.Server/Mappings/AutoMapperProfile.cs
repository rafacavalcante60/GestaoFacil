using AutoMapper;
using GestaoFacil.Server.Models.Principais;
using GestaoFacil.Server.Models.Usuario;
using GestaoFacil.Shared.DTOs.Auth;
using GestaoFacil.Shared.DTOs.Despesa;
using GestaoFacil.Shared.DTOs.Financeiro;
using GestaoFacil.Shared.DTOs.Usuario;

namespace GestaoFacil.Server.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            //despesa
            CreateMap<DespesaModel, DespesaDto>()
                .ForMember(dest => dest.NomeUsuario, opt => opt.MapFrom(src => src.Usuario.Nome));

            CreateMap<DespesaCreateDto, DespesaModel>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UsuarioId, opt => opt.Ignore())
                .ForMember(dest => dest.Usuario, opt => opt.Ignore());

            CreateMap<DespesaUpdateDto, DespesaModel>()
                .ForMember(dest => dest.UsuarioId, opt => opt.Ignore())
                .ForMember(dest => dest.Usuario, opt => opt.Ignore());

            //receita
            CreateMap<ReceitaModel, ReceitaDto>()
                .ForMember(dest => dest.NomeUsuario, opt => opt.MapFrom(src => src.Usuario.Nome));

            CreateMap<ReceitaCreateDto, ReceitaModel>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UsuarioId, opt => opt.Ignore())
                .ForMember(dest => dest.Usuario, opt => opt.Ignore());

            CreateMap<ReceitaUpdateDto, ReceitaModel>()
                .ForMember(dest => dest.UsuarioId, opt => opt.Ignore())
                .ForMember(dest => dest.Usuario, opt => opt.Ignore());

            //usuario
            CreateMap<UsuarioModel, UsuarioDto>();

            CreateMap<UsuarioUpdateDto, UsuarioModel>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.TipoUsuarioId, opt => opt.Ignore())
                .ForMember(dest => dest.TipoUsuario, opt => opt.Ignore());

            CreateMap<UsuarioAdminUpdateDto, UsuarioModel>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) 
                .ForMember(dest => dest.TipoUsuario, opt => opt.Ignore()); 

            CreateMap<UsuarioRegisterDto, UsuarioModel>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.SenhaHash, opt => opt.Ignore()) 
                .ForMember(dest => dest.TipoUsuario, opt => opt.Ignore());

            CreateMap<UsuarioRegisterDto, UsuarioModel>()
    .ForMember(dest => dest.Id, opt => opt.Ignore())          // Ignora o Id (gerado pelo DB)
    .ForMember(dest => dest.SenhaHash, opt => opt.Ignore())   // Ignora a senha hash (vai setar manualmente no Service)
    .ForMember(dest => dest.TipoUsuario, opt => opt.Ignore()); // Ignora a navegação TipoUsuario, será setado pelo TipoUsuarioId

        }
    }
}
