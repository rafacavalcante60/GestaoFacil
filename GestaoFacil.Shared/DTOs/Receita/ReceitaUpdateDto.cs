using System.ComponentModel.DataAnnotations;

namespace GestaoFacil.Shared.DTOs.Receita
{
    public class ReceitaUpdateDto : ReceitaCreateDto
    {
        [Required]
        public int Id { get; set; }
    }
}
