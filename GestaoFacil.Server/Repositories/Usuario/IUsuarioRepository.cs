﻿using GestaoFacil.Server.Models.Usuario;

public interface IUsuarioRepository
{
    Task<UsuarioModel?> GetByIdAsync(int id);
    Task<List<UsuarioModel>> GetRecentAsync();
    Task AddAsync(UsuarioModel usuario);
    Task UpdateAsync(UsuarioModel usuario);
    Task DeleteAsync(UsuarioModel usuario);
    Task<UsuarioModel?> GetByEmailAsync(string email);
    Task<bool> EmailExistsAsync(string email);
    Task<TipoUsuarioModel?> GetTipoUsuarioByNameAsync(string nome);
}
