using Microsoft.EntityFrameworkCore;
using SmartFridgeAPI.Models;
using System.Collections.Generic;

namespace SmartFridgeAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Это наша таблица в базе данных
        public DbSet<SavedRecipe> SavedRecipes { get; set; }
    }
}