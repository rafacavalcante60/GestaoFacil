﻿namespace GestaoFacil.Shared.DTOs.Auth
{
    public class RegisterRequest
{
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
    }
}