using CookingRecipes.Data;
using CookingRecipes.Services.Implementations;
using CookingRecipes.Services.Interfaces;
using CookingRecipes.Web;
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

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<CookingRecipesDbContext>();
builder.Services.AddControllersWithViews();



builder.Services.AddScoped<IRecipeService, RecipeService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();

var app = builder.Build();

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
