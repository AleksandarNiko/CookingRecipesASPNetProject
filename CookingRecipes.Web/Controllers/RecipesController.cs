using Microsoft.AspNetCore.Mvc;
using CookingRecipes.Services.Interfaces;
using CookingRecipes.Data.Models;
using CookingRecipes.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace CookingRecipes.Web.Controllers
{
    public class RecipesController : Controller
    {
        private readonly IRecipeService recipeService;
        private readonly CookingRecipesDbContext context;
        private readonly ILogger<RecipesController> logger;
        private readonly UserManager<IdentityUser> userManager;

        // Inject-ваме и Service и DbContext (DbContext за Categories засега)
        public RecipesController(
            IRecipeService recipeService,
            CookingRecipesDbContext context,
            ILogger<RecipesController> logger,
            UserManager<IdentityUser> userManager)
        {
            this.recipeService = recipeService;
            this.context = context;
            this.logger = logger;
            this.userManager = userManager;
        }

        // GET: Recipes
        public async Task<IActionResult> Index()
        {
            var recipes = await recipeService.GetAllAsync();

            return View(recipes);
        }

        // GET: Recipes/Create
        [Authorize]
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await context.Categories.ToListAsync();

            return View();
        }

        // POST: Recipes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
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

            // If the user is authenticated, map Identity user to domain User (create if missing)
            if (User?.Identity?.IsAuthenticated ?? false)
            {
                var identityUser = await userManager.GetUserAsync(User);
                var email = identityUser?.Email ?? identityUser?.UserName;

                var domainUser = await context.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (domainUser == null)
                {
                    domainUser = new CookingRecipes.Data.Models.User
                    {
                        Username = identityUser?.UserName ?? email ?? "user",
                        Email = email ?? string.Empty,
                        PasswordHash = string.Empty,
                        Role = "User",
                        CreatedAt = DateTime.UtcNow
                    };

                    context.Users.Add(domainUser);
                    await context.SaveChangesAsync();
                }

                recipe.CreatedBy = domainUser.UserID;
            }
            else
            {
                // If anonymous somehow allowed, fallback to a system user
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
            }

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
