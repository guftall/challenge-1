using System;
using System.Linq;
using HiliTechChallenge.Core;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace HiliTechChallenge.Controllers
{
    public class HiliController : Controller
    {
        protected readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };
        
        protected string GetCookieToken()
        {
            var cookieToken = User.Claims.First(c => c.Type.Equals("access_token"));
            return cookieToken.Value;
        }

        protected Guid CurrentUserId()
        {
            var claims = Utilities.JwtTokenClaims(GetCookieToken());
            return Guid.Parse(claims.First(c => c.Type == "sub").Value);
        }
    }
}