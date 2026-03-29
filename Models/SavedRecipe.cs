using System;

namespace SmartFridgeAPI.Models // Проверь, чтобы Namespace был как в проекте
{
    public class Recipe
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime SavedAt { get; set; } = DateTime.Now;
    }
}