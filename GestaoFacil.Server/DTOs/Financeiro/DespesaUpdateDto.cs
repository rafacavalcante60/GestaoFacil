using System;
using System.ComponentModel.DataAnnotations;

namespace GestaoFacil.Server.DTOs.Despesa
{
    public class DespesaUpdateDto : DespesaCreateDto
    {
        [Required]
        public int Id { get; set; }
    }
}