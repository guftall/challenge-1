using System.ComponentModel.DataAnnotations;
using HiliTechChallenge.Core.Enums;

namespace HiliTechChallenge.Core.Models
{
    public class AdvertiseStatusUpdateApiModel
    {
        [Required]
        public AdsStatus Status { get; set; }
    }
}