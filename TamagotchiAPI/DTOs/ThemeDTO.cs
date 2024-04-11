using System.ComponentModel.DataAnnotations;

namespace TamagotchiAPI.DTOs;

public class ThemeDTO
{
    [Required]
    public int ThemeId { get; set; }
}