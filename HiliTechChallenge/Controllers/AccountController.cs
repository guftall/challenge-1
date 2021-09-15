using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using HiliTechChallenge.Core;
using HiliTechChallenge.Core.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace HiliTechChallenge.Controllers
{
    [Route("/account")]
    public class AccountController : HiliController
    {
        private readonly AuthHelper _authHelper;

        public AccountController(AuthHelper authHelper)
        {
            _authHelper = authHelper;
        }

        [HttpGet("login")]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                if (User.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == "admin"))
                {
                    return Redirect("/Admin");
                }
                if (User.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == "advertiser"))
                {
                    return Redirect("/Advertiser");
                }
            }
            return View();
        }

        [HttpPost("login")]
        public async Task<IActionResult> PostLogin([FromForm, Required] LoginViewModel loginViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var normalizedEmail = loginViewModel.Email.ToLower();
            var token = await _authHelper.GetAccessToken(loginViewModel.Email, loginViewModel.Password, loginViewModel.AsAdmin);

            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("username or password is wrong");
            }
            
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, normalizedEmail),
                new Claim(ClaimTypes.Email, normalizedEmail),
                new Claim("access_token", token),
                new Claim(ClaimTypes.Role, loginViewModel.AsAdmin ? "admin" : "advertiser"),
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme, 
                new ClaimsPrincipal(claimsIdentity), 
                authProperties);

            var redirectUri = loginViewModel.AsAdmin
                ? "http://admin.hili.guftall.ir"
                : "http://user.hili.guftall.ir";
            
            return Redirect(redirectUri);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return Redirect("/");
        }
    }
}