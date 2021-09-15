using System.ComponentModel.DataAnnotations;

namespace HiliTechChallenge.Core.Models
{
    public class LoginViewModel
    {
        [Required, EmailAddress]
        public string Email { get; set; }
        
        [Required, MinLength(4)]
        public string Password { get; set; }
        
        [Required]
        public bool AsAdmin { get; set; }
    }
}