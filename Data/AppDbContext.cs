using Microsoft.EntityFrameworkCore;
using SmartFridgeAPI.Models; // ОБЯЗАТЕЛЬНО: импорт папки с моделью Recipe

namespace SmartFridgeAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // ВОТ ЭТА СТРОКА РЕШАЕТ ТВОЮ ОШИБКУ:
        public DbSet<Recipe> Recipes { get; set; }
    }
}