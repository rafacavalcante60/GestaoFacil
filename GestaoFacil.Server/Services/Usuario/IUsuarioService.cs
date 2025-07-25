﻿using GestaoFacil.Server.DTOs.Usuario;
using GestaoFacil.Server.Responses;

namespace GestaoFacil.Server.Services.Usuario
{
    public interface IUsuarioService
    {
        Task<ResponseModel<UsuarioDto>> GetByIdAsync(int id);
        Task<ResponseModel<List<UsuarioDto>>> GetRecentAsync();
        Task<ResponseModel<bool>> UpdatePerfilAsync(int id, UsuarioUpdateDto dto);
        Task<ResponseModel<bool>> UpdateAdminAsync(int id, UsuarioAdminUpdateDto dto);
        Task<ResponseModel<bool>> DeleteAdminAsync(int id);
    }
}
