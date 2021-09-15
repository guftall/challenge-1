using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HiliTechChallenge.Core.Models.Entities
{
    public class CategoryEntity
    {
        [Key]
        public int Id { get; set; }
        
        public string Name { get; set; }
        
        public int? ParentCategoryId { get; set; }
        public CategoryEntity ParentCategoryEntity { get; set; }
        
        public List<CategoryEntity> SubCategories { get; set; }
        public List<AdsEntity> Advertises { get; set; }
    }
}