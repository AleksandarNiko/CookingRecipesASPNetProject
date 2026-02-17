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
        private readonly ILogger<RecipesController> logger;

        // Inject-ваме и Service и DbContext (DbContext за Categories засега)
        public RecipesController(
            IRecipeService recipeService,
            CookingRecipesDbContext context,
            ILogger<RecipesController> logger)
        {
            this.recipeService = recipeService;
            this.context = context;
            this.logger = logger;
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
            int? SelectedCategoryId)
        {
            if (!ModelState.IsValid)
            {
                // Log model state errors to help diagnose binding/validation issues
                logger.LogWarning("ModelState invalid when creating recipe.");
                foreach (var kv in ModelState)
                {
                    foreach (var err in kv.Value.Errors)
                    {
                        logger.LogWarning("ModelState error for {Field}: {Error}", kv.Key, err.ErrorMessage);
                    }
                }

                ViewBag.Categories = await context.Categories.ToListAsync();
                return View(recipe);
            }

            try
            {

            // Ensure CreatedAt is set
            recipe.CreatedAt = DateTime.UtcNow;

            // Ensure there is a valid CreatedBy user. If none exists, create a fallback system user.
            var user = await context.Users.FirstOrDefaultAsync();
            if (user == null)
            {
                user = new CookingRecipes.Data.Models.User
                {
                    Username = "system",
                    Email = "system@local",
                    PasswordHash = string.Empty,
                    Role = "Admin",
                    CreatedAt = DateTime.UtcNow
                };

                context.Users.Add(user);
                await context.SaveChangesAsync();
            }

            recipe.CreatedBy = user.UserID;

            var selected = SelectedCategoryId.HasValue
                ? new List<int> { SelectedCategoryId.Value }
                : new List<int>();

            await recipeService.CreateAsync(recipe, selected);

            logger.LogInformation("Recipe created: {Title} (ID will be set after save)", recipe.Title);

            return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating recipe");
                ViewBag.Categories = await context.Categories.ToListAsync();
                ModelState.AddModelError(string.Empty, "An error occurred while creating the recipe.");
                return View(recipe);
            }
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
