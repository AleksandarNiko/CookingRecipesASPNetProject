using Microsoft.AspNetCore.Mvc;
using CookingRecipes.Services.Interfaces;
using CookingRecipes.Data.Models;

namespace CookingRecipes.Web.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryService categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            this.categoryService = categoryService;
        }

        // GET: Category
        public async Task<IActionResult> Index()
        {
            var categories = await categoryService.GetAllAsync();

            return View(categories);
        }

        // GET: Category/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Category/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (!ModelState.IsValid)
                return View(category);

            await categoryService.CreateAsync(category);

            return RedirectToAction(nameof(Index));
        }
    }
}
