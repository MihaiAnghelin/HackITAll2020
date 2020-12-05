using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HackItApi.Models
{
    public class User
    {
        [Required]public string Id { get; set; }
        [Required]public string FirstName { get; set; }
        [Required]public string LastName { get; set; }
        [Required]public string Email { get; set; }
        [Required][JsonIgnore]public string Password { get; set; }
        public decimal Balance { get; set; }
        [Required]public DateTime CreatedDate { get; set; }
    }
}