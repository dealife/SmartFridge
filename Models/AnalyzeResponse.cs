using System.Collections.Generic;

namespace SmartFridgeAPI.Models
{
    // Это основной объект ответа
    public class AnalyzeResponse
    {
        // Список продуктов, которые AI нашел в холодильнике
        public List<string> Ingredients { get; set; } = new();

        // Список рецептов, которые предложил AI
        public List<Recipe> Recipes { get; set; } = new();
    }

    // Это описание одного конкретного рецепта
    public class Recipe
    {
        public string Title { get; set; } // Название блюда
        public string Description { get; set; } // Краткое описание или шаги

        // Список продуктов, которых НЕ хватает (надо купить)
        public List<string> MissingIngredients { get; set; } = new();
    }
}