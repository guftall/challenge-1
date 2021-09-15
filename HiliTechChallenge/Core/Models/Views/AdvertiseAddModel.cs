using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace HiliTechChallenge.Core.Models.Views
{
    public class AdvertiseAddModel
    {
        public string Title { get; set; }
        public string Text { get; set; }
        public string Contact { get; set; }
        public int CategoryId { get; set; }
        public List<IFormFile> Images { get; set; }
        public List<string> ImageUrls { get; set; }
    }
}