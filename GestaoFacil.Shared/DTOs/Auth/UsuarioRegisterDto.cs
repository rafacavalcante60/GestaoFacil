﻿using System.ComponentModel.DataAnnotations;

namespace GestaoFacil.Shared.DTOs.Auth
{
    public class UsuarioRegisterDto
    {
        [Required, StringLength(100)]
        public string Nome { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, MinLength(6)]
        public string Senha { get; set; } = string.Empty;
    }
}