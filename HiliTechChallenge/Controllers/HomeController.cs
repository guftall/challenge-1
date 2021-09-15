using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using HiliTechChallenge.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using HiliTechChallenge.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace HiliTechChallenge.Controllers
{
    public class HomeController : HiliController
    {
        private readonly AdsDbContext _dbContext;

        public HomeController(AdsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _dbContext.Categories
                .Where(c => c.ParentCategoryId == null).ToListAsync();
            ViewBag.Categories = JsonConvert.SerializeObject(categories, Formatting.Indented, _jsonSerializerSettings);
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}