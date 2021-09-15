using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HiliTechChallenge.Core;
using HiliTechChallenge.Core.Enums;
using HiliTechChallenge.Core.Models;
using HiliTechChallenge.Core.Models.Entities;
using HiliTechChallenge.Core.Models.Views;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace HiliTechChallenge.Controllers
{
    [Authorize]
    [Route("/advertise")]
    public class AdvertiseController : HiliController
    {
        private readonly AdsDbContext _dbContext;

        public AdvertiseController(AdsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewBag.Advertises =
                await _dbContext.Advertises
                    .Where(a => a.UserId.Equals(CurrentUserId()))
                    .OrderByDescending(a => a.Id)
                    .Include(a => a.CategoryEntity).ToListAsync();
            return View();
        }

        [HttpGet("search")]
        [AllowAnonymous]
        public async Task<IActionResult> SearchCategories([FromQuery] int? categoryId, [FromQuery] int offset = 0,
            [FromQuery] int count = 5)
        {
            var q = _dbContext.Advertises.Where(a => a.Status == AdsStatus.Published);
            if (categoryId.HasValue)
            {
                var ids = await CategoryTreeIds(categoryId.Value);
                q = q.Where(c => ids.Contains(c.CategoryId));
            }

            var advertises = await q
                .OrderByDescending(a => a.Id)
                .Skip(offset * count)
                .Take(count)
                .Select(a => new
                {
                    a.Id,
                    a.Title,
                    a.Text,
                    a.ContactInfo,
                    a.CategoryId,
                    categoryName = a.CategoryEntity.Name,
                    a.ImageUrls,
                })
                .ToListAsync();

            return Ok(advertises);
        }

        [HttpGet("add")]
        public async Task<IActionResult> GetAdd()
        {
            ViewBag.Action = "add";
            var categories = await _dbContext.Categories.Where(c => c.ParentCategoryId == null).ToListAsync();
            ViewBag.Categories = JsonConvert.SerializeObject(categories, Formatting.Indented, _jsonSerializerSettings);
            return View("Add");
        }

        [HttpPost("add")]
        public async Task<IActionResult> PostAdd([FromForm, Required] AdvertiseAddModel model)
        {
            var imageUrls = new List<string>();
            if (model.Images != null)
            {
                foreach (var formFile in model.Images)
                {
                    if (formFile.Length > 0)
                    {
                        var filePath = $"{Path.GetTempPath()}{formFile.FileName}";

                        imageUrls.Add(filePath);
                        using (var stream = System.IO.File.Create(filePath))
                        {
                            await formFile.CopyToAsync(stream);
                        }
                    }
                }
            }

            _dbContext.Advertises.Add(new AdsEntity
            {
                UserId = CurrentUserId(),
                Title = model.Title,
                Text = model.Text,
                Status = AdsStatus.Published,
                CategoryId = model.CategoryId,
                ContactInfo = model.Contact,
                ImageUrls = imageUrls,
            });

            await _dbContext.SaveChangesAsync();

            ViewBag.Categories = await _dbContext.Categories.Where(c => c.ParentCategoryId == null).ToListAsync();
            return RedirectToAction("Index");
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAdvertise([FromRoute] int id)
        {
            var ads = await _dbContext.Advertises
                .Where(a => a.UserId.Equals(CurrentUserId()) && a.Id == id)
                .Include(a => a.CategoryEntity)
                .FirstAsync();

            ViewBag.Action = "edit";
            ViewBag.Category = ads.CategoryEntity;
            var rootCategories = await _dbContext.Categories.Where(c => c.ParentCategoryId == null).ToListAsync();
            var categoryWithPathToRoot = await ParentCategoryPath(ads.CategoryId);
            var rootParentCategory = RootParentCategory(categoryWithPathToRoot);

            rootCategories[rootCategories.FindIndex(c => c.Id == rootParentCategory.Id)] = rootParentCategory;

            var categoriesString =
                JsonConvert.SerializeObject(rootCategories, Formatting.Indented, _jsonSerializerSettings);
            ViewBag.Categories = categoriesString;
            ViewBag.Images = ads.ImageUrls;

            var viewModel = new AdvertiseAddModel
            {
                Title = ads.Title,
                Text = ads.Text,
                Contact = ads.ContactInfo,
                CategoryId = ads.CategoryId,
            };
            return View("Add", viewModel);
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> Edit([FromRoute] int id, [Required, FromForm] AdvertiseAddModel model)
        {
            var ads = await _dbContext.Advertises
                .Where(a => a.UserId.Equals(CurrentUserId()) && a.Id == id)
                .FirstAsync();

            var imageUrls = new List<string>();
            if (model.ImageUrls != null)
            {
                imageUrls.AddRange(model.ImageUrls);
            }

            if (model.Images != null)
            {
                foreach (var formFile in model.Images)
                {
                    if (formFile.Length > 0)
                    {
                        var filePath = $"{Path.GetTempPath()}{formFile.FileName}";

                        imageUrls.Add(filePath);
                        using (var stream = System.IO.File.Create(filePath))
                        {
                            await formFile.CopyToAsync(stream);
                        }
                    }
                }
            }

            ads.Title = model.Title;
            ads.Text = model.Text;
            ads.ContactInfo = model.Contact;
            ads.ImageUrls = imageUrls;
            ads.CategoryId = model.CategoryId;

            _dbContext.Update(ads);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var ads = await _dbContext.Advertises
                .Where(a => a.UserId.Equals(CurrentUserId()) && a.Id == id)
                .FirstAsync();

            _dbContext.Remove(ads);
            await _dbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> SetStatus([FromRoute] int id,
            [Required, FromBody] AdvertiseStatusUpdateApiModel apiModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var ads = await _dbContext.Advertises
                .Where(a => a.UserId.Equals(CurrentUserId()) && a.Id == id).FirstAsync();

            ads.Status = apiModel.Status;
            _dbContext.Advertises.Update(ads);
            await _dbContext.SaveChangesAsync();

            return Ok();
        }

        private async Task<CategoryEntity> ParentCategoryPath(int catId)
        {
            var category = await _dbContext.Categories
                .Include(c => c.SubCategories)
                .FirstAsync(c => c.Id == catId);

            if (category.ParentCategoryId.HasValue)
            {
                var parentCategory = await ParentCategoryPath(category.ParentCategoryId.Value);
                var catIndexInParentSubCategories = parentCategory.SubCategories.FindIndex(c => c.Id == category.Id);
                parentCategory.SubCategories[catIndexInParentSubCategories] = category;
            }

            return category;
        }

        private CategoryEntity RootParentCategory(CategoryEntity category)
        {
            if (category.ParentCategoryId.HasValue)
            {
                return RootParentCategory(category.ParentCategoryEntity);
            }

            return category;
        }

        private async Task<List<int>> CategoryTreeIds(int rootId)
        {
            var ids = new List<int>
            {
                rootId
            };

            var cat = await _dbContext.Categories
                .Include(c => c.SubCategories)
                .FirstAsync(c => c.Id == rootId);

            var tasks = cat.SubCategories.Select(subCat => CategoryTreeIds(subCat.Id)).ToList();

            await Task.WhenAll(tasks);

            foreach (var t in tasks)
            {
                ids.AddRange(t.Result);
            }

            return ids;
        }
    }
}