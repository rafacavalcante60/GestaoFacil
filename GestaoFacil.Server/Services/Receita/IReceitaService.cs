﻿using GestaoFacil.Shared.Responses;
using GestaoFacil.Shared.DTOs.Receita;

namespace GestaoFacil.Server.Services.Receita
{
    public interface IReceitaService
    {
        Task<ResponseModel<List<ReceitaDto>>> GetAllByUsuarioAsync(int usuarioId);
        Task<ResponseModel<ReceitaDto?>> GetByIdAsync(int id, int usuarioId);
        Task<ResponseModel<ReceitaDto>> CreateAsync(ReceitaCreateDto dto, int usuarioId);
        Task<ResponseModel<bool>> UpdateAsync(int id, ReceitaUpdateDto dto, int usuarioId);
        Task<ResponseModel<bool>> DeleteAsync(int id, int usuarioId);
    }
}
