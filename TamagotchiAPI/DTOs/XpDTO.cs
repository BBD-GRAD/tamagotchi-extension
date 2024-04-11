using System.ComponentModel.DataAnnotations;

namespace TamagotchiAPI.DTOs;

public class XpDTO
{
    [Required]
    public long XP { get; set; }
}