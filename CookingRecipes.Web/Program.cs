using CookingRecipes.Data;
using CookingRecipes.Services.Implementations;
using CookingRecipes.Services.Interfaces;
using CookingRecipes.Web;
using CookingRecipes.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using CookingRecipes.Services.Interfaces;
using CookingRecipes.Services.Implementations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<CookingRecipesDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Identity stores in a separate Identity DbContext to isolate identity schema
builder.Services.AddDbContext<ApplicationIdentityDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationIdentityDbContext>()
    .AddDefaultUI();

// Razor Pages (Identity UI)
builder.Services.AddRazorPages();

// Authorization policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdmin", policy => policy.RequireRole("Administrator"));
});
builder.Services.AddControllersWithViews();



builder.Services.AddScoped<IRecipeService, RecipesService>();
builder.Services.AddScoped<ICategoryService, CategoriesService>();

// Seed roles/admin on startup
// Make RoleInitializer available after Identity services
builder.Services.AddScoped<RoleInitializer>();

var app = builder.Build();

// Ensure databases exist (development convenience) and initialize roles/admin user
using (var scope = app.Services.CreateScope())
{
    var identityDb = scope.ServiceProvider.GetRequiredService<ApplicationIdentityDbContext>();
    var appDb = scope.ServiceProvider.GetRequiredService<CookingRecipesDbContext>();

    // Apply any pending migrations. This ensures Identity tables (e.g. AspNetRoles) are created.
    // In production use explicit migrations and avoid EnsureCreated.
    // Apply migrations for the Identity DbContext so Identity tables are created.
    // If there are no migrations or migration application fails, fall back to EnsureCreated.
    try
    {
        identityDb.Database.Migrate();
    }
    catch (Exception)
    {
        identityDb.Database.EnsureCreated();
    }

    // For the application DbContext use EnsureCreated to avoid requiring migrations during dev run.
    // In production, create and apply migrations for CookingRecipesDbContext instead.
    appDb.Database.EnsureCreated();

    var initializer = scope.ServiceProvider.GetRequiredService<RoleInitializer>();
    try
    {
        await initializer.InitializeAsync();
    }
    catch (Exception ex)
    {
        // Log and continue startup so the process doesn't crash if the identity schema isn't ready.
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while initializing roles.");
    }
}

// Log the bound addresses on application start to help diagnose which URLs Kestrel is listening on.
var _logger = app.Services.GetRequiredService<ILogger<Program>>();
app.Lifetime.ApplicationStarted.Register(() =>
{
    var addresses = app.Urls;
    var message = addresses.Count == 0
        ? "No URLs configured in app.Urls"
        : $"Now listening on: {string.Join(", ", addresses)}";

    _logger.LogInformation(message);
});

// Log configured endpoints to help diagnose routing (which routes/controllers are registered).
var endpointDataSource = app.Services.GetRequiredService<EndpointDataSource>();
foreach (var endpoint in endpointDataSource.Endpoints)
{
    _logger.LogInformation("Endpoint: {endpoint}", endpoint.DisplayName ?? endpoint.ToString());
}

// Simple request/response logging middleware to show incoming paths and response status codes.
app.Use(async (context, next) =>
{
    _logger.LogInformation("Request: {method} {path}", context.Request.Method, context.Request.Path + context.Request.QueryString);
    await next();
    _logger.LogInformation("Response: {statusCode} for {path}", context.Response.StatusCode, context.Request.Path);
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// Normalize repeated slashes in request path (e.g. convert '//' to '/') and redirect to normalized URL.
app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value;
    if (!string.IsNullOrEmpty(path) && path.Contains("//"))
    {
        var normalized = Regex.Replace(path, "/{2,}", "/");
        var newUrl = normalized + context.Request.QueryString;
        context.Response.Redirect(newUrl, permanent: false);
        return;
    }

    await next();
});

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
