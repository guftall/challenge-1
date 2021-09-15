using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using HiliTechChallenge.Core;
using HiliTechChallenge.Core.Models;
using HiliTechChallenge.Core.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HiliTechChallenge.Controllers
{
    [Route("/category")]
    public class CategoryController : Controller
    {
        private readonly AdsDbContext _dbContext;

        public CategoryController(AdsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories([FromQuery] int? parentId)
        {
            var cats = await _dbContext.Categories.Where(c => c.ParentCategoryId == parentId).ToListAsync();

            return Ok(cats);
        }

        [HttpPost]
        [Authorize(Policy = "admin")]
        public async Task<IActionResult> AddCategory([FromForm, Required] CategoryAddApiModel categoryAddApiModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (categoryAddApiModel.ParentId.HasValue)
            {
                var parentExists =
                    await _dbContext.Categories.AnyAsync(c => c.Id == categoryAddApiModel.ParentId.Value);

                if (!parentExists)
                {
                    return BadRequest(new {error = "parent cat not exists"});
                }
            }
            
            var duplicateNameExists =
                await _dbContext.Categories.AnyAsync(c => c.ParentCategoryId == categoryAddApiModel.ParentId && c.Name.Equals(categoryAddApiModel.Name));

            if (duplicateNameExists)
            {
                return BadRequest(new {error = "duplicate category"});
            }

            var cat = _dbContext.Categories.Add(new CategoryEntity
            {
                Name = categoryAddApiModel.Name,
                ParentCategoryId = categoryAddApiModel.ParentId
            });

            await _dbContext.SaveChangesAsync();

            return Ok(cat.Entity);
        }
    }
}