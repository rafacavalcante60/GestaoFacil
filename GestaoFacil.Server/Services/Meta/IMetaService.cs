using GestaoFacil.Server.DTOs.Meta;
using GestaoFacil.Server.Responses;

namespace GestaoFacil.Server.Services.Meta
{
    public interface IMetaService
    {
        Task<ResponseModel<List<MetaDto>>> GetByUsuarioAsync(int usuarioId);
        Task<ResponseModel<List<MetaDto>>> GetAtivasAsync(int usuarioId);
        Task<ResponseModel<MetaDto?>> GetByIdAsync(int id, int usuarioId);
        Task<ResponseModel<MetaDto>> CreateAsync(MetaCreateDto dto, int usuarioId);
        Task<ResponseModel<bool>> UpdateAsync(int id, MetaUpdateDto dto, int usuarioId);
        Task<ResponseModel<bool>> DeleteAsync(int id, int usuarioId);
    }
}
