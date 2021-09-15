using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using HiliTechChallenge.Core;
using HiliTechChallenge.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HiliTechChallenge.Controllers
{
    [Authorize(Policy = "admin")]
    [Route("/user")]
    public class UserController : HiliController
    {
        private readonly AuthHelper _authHelper;
        
        public UserController(AuthHelper authHelper)
        {
            _authHelper = authHelper;
        }
        // GET
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult User()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddUser([FromForm, Required] AdminPageViewModel adminPageViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _authHelper.AddUser(GetCookieToken(), adminPageViewModel.Email, adminPageViewModel.Password);

            return View("User");
        }
    }
}