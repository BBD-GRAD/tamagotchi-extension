using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TamagotchiAPI.Models;

[Table("Themes")]
public class Theme
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int ThemeId { get; set; }
    
    [Required]
    public string ThemeName { get; set; }
}