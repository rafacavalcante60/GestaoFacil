using System.ComponentModel.DataAnnotations;

namespace GestaoFacil.Shared.DTOs.Financeiro
{
    public class ReceitaUpdateDto : ReceitaCreateDto
    {
        [Required]
        public int Id { get; set; }
    }
}
