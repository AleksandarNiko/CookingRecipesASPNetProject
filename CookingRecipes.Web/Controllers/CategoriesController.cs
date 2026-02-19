using Microsoft.AspNetCore.Mvc;
using CookingRecipes.Services.Interfaces;
using CookingRecipes.Data.Models;
using CookingRecipes.Data;
using Microsoft.EntityFrameworkCore;

namespace CookingRecipes.Web.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Administrator")]
    public class CategoriesController : Controller
    {
        private readonly ICategoryService categoryService;
        private readonly CookingRecipesDbContext context;

        public CategoriesController(ICategoryService categoryService, CookingRecipesDbContext context)
        {
            this.categoryService = categoryService;
            this.context = context;
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

        // GET: Categories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var category = await context.Categories.FirstOrDefaultAsync(c => c.CategoryID == id.Value);
            if (category == null)
                return NotFound();

            return View(category);
        }

        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var category = await context.Categories.FindAsync(id.Value);
            if (category == null)
                return NotFound();

            return View(category);
        }

        // POST: Categories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Category category)
        {
            if (id != category.CategoryID)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(category);

            try
            {
                context.Update(category);
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await context.Categories.AnyAsync(e => e.CategoryID == id))
                    return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Categories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var category = await context.Categories.FirstOrDefaultAsync(c => c.CategoryID == id.Value);
            if (category == null)
                return NotFound();

            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await context.Categories.FindAsync(id);
            if (category != null)
            {
                context.Categories.Remove(category);
                await context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
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
