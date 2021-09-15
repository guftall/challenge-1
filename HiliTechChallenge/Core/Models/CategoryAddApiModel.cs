using System.ComponentModel.DataAnnotations;

namespace HiliTechChallenge.Core.Models
{
    public class CategoryAddApiModel
    {
        [Required, MinLength(1)]
        public string Name { get; set; }
        
        public int? ParentId { get; set; }
    }
}