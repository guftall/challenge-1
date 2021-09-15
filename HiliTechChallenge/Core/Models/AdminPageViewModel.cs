using HiliTechChallenge.Core.Models.Views;

namespace HiliTechChallenge.Core.Models
{
    public class AdminPageViewModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        
        public CategoryBoxViewModel CategoryBoxViewModel { get; set; }
    }
}