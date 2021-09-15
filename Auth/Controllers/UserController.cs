using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using Auth.Data;
using Auth.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Auth.Controllers
{
    [Route("user")]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        
        public UserController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> AddUser([Required, FromBody] UserAddApiModel userAddApiModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userExists = await _userManager.Users.AnyAsync(u => u.Email.Equals(userAddApiModel.Email.ToLower()));

            if (userExists)
            {
                return BadRequest(new {error = "user exists"});
            }
            
            var user = new ApplicationUser
            {
                UserName = userAddApiModel.Email.ToLower(),
                Email = userAddApiModel.Email.ToLower(),
                CreatedAt = DateTime.Now
            };
            

            var createUserResult = await _userManager.CreateAsync(user, userAddApiModel.Password);
            if (createUserResult.Succeeded)
            {
                
                await _userManager.AddClaimAsync(user, new Claim("role", "advertiser"));
                return Ok();
            }
            
            return BadRequest(new { errors = createUserResult.Errors });
        }
    }
}