using System.ComponentModel.DataAnnotations;

namespace HackItApi.Models
{
    public class FavStonks
    {
        [Required]public string Id { get; set; }
        [Required]public string UserId { get; set; }
        [Required]public string Symbol { get; set; }
    }
}