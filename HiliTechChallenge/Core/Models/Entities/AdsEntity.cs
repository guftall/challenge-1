using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using HiliTechChallenge.Core.Enums;

namespace HiliTechChallenge.Core.Models.Entities
{
    public class AdsEntity
    {
        [Key] public int Id { get; set; }
        public Guid UserId { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string ContactInfo { get; set; }

        public int CategoryId { get; set; }
        public CategoryEntity CategoryEntity { get; set; }

        public AdsStatus Status { get; set; }

        public List<string> ImageUrls { get; set; }

    }
}