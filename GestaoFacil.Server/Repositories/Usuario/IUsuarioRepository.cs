﻿using GestaoFacil.Server.Models;
using GestaoFacil.Server.Models.Principais;

public interface IUsuarioRepository
{
    Task<UsuarioModel?> GetByIdAsync(int id);
    Task<List<UsuarioModel>> GetAllAsync();
    Task AddAsync(UsuarioModel usuario);
    Task UpdateAsync(UsuarioModel usuario);
    Task DeleteAsync(UsuarioModel usuario);
    Task<UsuarioModel?> GetByEmailAsync(string email);
    Task<bool> EmailExistsAsync(string email);
    Task<TipoUsuarioModel?> GetTipoUsuarioByNameAsync(string nome);
}
