using AutoMapper;
using GestaoFacil.Shared.DTOs.Despesa;
using GestaoFacil.Shared.DTOs.Receita;
using GestaoFacil.Shared.DTOs.Usuario;

using AutoMapper;
using GestaoFacil.Shared.DTOs.Despesa;
using GestaoFacil.Shared.DTOs.Receita;
using GestaoFacil.Shared.DTOs.Usuario;
using GestaoFacil.Server.Models.Principais;

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

            CreateMap<UsuarioModel, UsuarioDto>();

            // Atualização do próprio usuário: pode editar nome e email, mas NÃO tipoUsuario nem Id
            CreateMap<UsuarioUpdateDto, UsuarioModel>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.TipoUsuarioId, opt => opt.Ignore())
                .ForMember(dest => dest.TipoUsuario, opt => opt.Ignore());

            // Atualização feita pelo Admin: pode alterar nome, email e tipoUsuario (via Id)
            CreateMap<UsuarioAdminUpdateDto, UsuarioModel>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) // Id deve vir via rota/parametro, não do DTO
                .ForMember(dest => dest.TipoUsuario, opt => opt.Ignore()) // entidade não mapeia direto da string
                .ForMember(dest => dest.TipoUsuarioId, opt => opt.MapFrom(src =>
                    src.TipoUsuario.Equals("Admin", System.StringComparison.OrdinalIgnoreCase) ? 2 : 1));
        }
    }
}
