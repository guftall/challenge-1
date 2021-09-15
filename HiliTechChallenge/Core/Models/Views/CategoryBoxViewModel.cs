using System.Collections.Generic;

namespace HiliTechChallenge.Core.Models.Views
{
    public class CategoryBoxViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<CategoryBoxViewModel> SubCategories { get; set; }
    }
}