using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using HiliTechChallenge.Core.Enums;

namespace HiliTechChallenge.Core
{
    public static class Utilities
    {
        public static IEnumerable<Claim> JwtTokenClaims(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            return jwtToken.Claims;
        }
        
        public static string AdsStatusStr(AdsStatus status)
        {
            switch (status)
            {
                case AdsStatus.Draft:
                    return "Draft";
                case AdsStatus.Published:
                    return "Published";
                default:
                    return "";
            }
        }
    }
}