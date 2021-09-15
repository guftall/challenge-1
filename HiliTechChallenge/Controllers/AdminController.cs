using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HiliTechChallenge.Core;
using HiliTechChallenge.Core.Models;
using HiliTechChallenge.Core.Models.Views;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HiliTechChallenge.Controllers
{
    [Authorize(Policy = "admin")]
    public class AdminController : HiliController
    {
        private readonly AdsDbContext _dbContext;
        public AdminController(AdsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new AdminPageViewModel
            {
                CategoryBoxViewModel = new CategoryBoxViewModel
                {
                    SubCategories = await _dbContext.Categories.Where(c => c.ParentCategoryId == null).Select(c => new CategoryBoxViewModel
                    {
                        Id = c.Id,
                        Name = c.Name
                    }).ToListAsync()
                        
                }
            };
            return View(viewModel);
        }
    }
}