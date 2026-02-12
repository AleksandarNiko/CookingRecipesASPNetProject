using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Text;

namespace CookingRecipes.Data
{
    internal class CookingRecipesDbContextFactory : IDesignTimeDbContextFactory<CookingRecipesDbContext>
    {
        public CookingRecipesDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<CookingRecipesDbContext>();

            optionsBuilder.UseSqlServer(
                "Server=(localdb)\\MSSQLLocalDB;Database=CookingRecipesDb;TrustServerCertificate=true;Trusted_Connection=True;");

            return new CookingRecipesDbContext(optionsBuilder.Options);
        }
    }
}
