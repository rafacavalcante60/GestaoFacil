﻿namespace GestaoFacil.Server.DTOs.Auth
{
    public class ResetPasswordRequestDto
    {
        public string Token { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
    }
}
