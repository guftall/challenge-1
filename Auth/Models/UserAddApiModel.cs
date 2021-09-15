using System.ComponentModel.DataAnnotations;

namespace Auth.Models
{
    public class UserAddApiModel
    {
        [Required, EmailAddress]
        public string Email { get; set; }
        
        [Required, MinLength(4)]
        public string Password { get; set; }
    }
}