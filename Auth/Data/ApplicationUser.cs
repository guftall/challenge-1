using System;
using Microsoft.AspNetCore.Identity;

namespace Auth.Data
{
    public class ApplicationUser : IdentityUser
    {
        public DateTime CreatedAt { get; set; }
    }
}