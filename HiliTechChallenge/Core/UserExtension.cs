using System.Security.Claims;

namespace HiliTechChallenge.Core
{
    public static class UserExtension
    {
        public static bool IsAdmin(this ClaimsPrincipal user)
        {
            return user.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == "admin");
        }
    }
}