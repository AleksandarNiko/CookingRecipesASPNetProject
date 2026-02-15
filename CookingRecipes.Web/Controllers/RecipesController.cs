using Microsoft.AspNetCore.Mvc;
using CookingRecipes.Services.Interfaces;
using CookingRecipes.Data.Models;
using CookingRecipes.Data;
using Microsoft.EntityFrameworkCore;

namespace CookingRecipes.Web.Controllers
{
    public class RecipesController : Controller
    {
        private readonly IRecipeService recipeService;
        private readonly CookingRecipesDbContext context;

        // Inject-ваме и Service и DbContext (DbContext за Categories засега)
        public RecipesController(
            IRecipeService recipeService,
            CookingRecipesDbContext context)
        {
            this.recipeService = recipeService;
            this.context = context;
        }

        // GET: Recipes
        public async Task<IActionResult> Index()
        {
            var recipes = await recipeService.GetAllAsync();

            return View(recipes);
        }

        // GET: Recipes/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await context.Categories.ToListAsync();

            return View();
        }

        // POST: Recipes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            Recipe recipe,
            List<int> SelectedCategoryIds)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await context.Categories.ToListAsync();
                return View(recipe);
            }

            await recipeService.CreateAsync(recipe, SelectedCategoryIds);

            return RedirectToAction(nameof(Index));
        }

        // GET: Recipes/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var recipe = await recipeService.GetByIdAsync(id);

            if (recipe == null)
                return NotFound();

            return View(recipe);
        }
    }
}
