﻿using System.ComponentModel.DataAnnotations;

namespace GestaoFacil.Server.DTOs.Financeiro
{
    public class ReceitaUpdateDto : ReceitaCreateDto
    {
        [Required]
        public int Id { get; set; }
    }
}
