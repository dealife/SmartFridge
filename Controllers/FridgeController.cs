using Microsoft.AspNetCore.Mvc;
using SmartFridgeAPI.Data;
using SmartFridgeAPI.Models;
using SmartFridgeAPI.Services;

namespace SmartFridgeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FridgeController : ControllerBase
    {
        private readonly GeminiService _geminiService = new GeminiService();
        private readonly AppDbContext _context;

        // Внедряем базу данных в контроллер
        public FridgeController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("analyze")]
        public async Task<IActionResult> AnalyzeFridge(IFormFile photo)
        {
            if (photo == null || photo.Length == 0) return BadRequest("Загрузите фото!");

            var result = await _geminiService.AnalyzeImage(photo);

            if (result == null)
            {
                return Ok(new AnalyzeResponse
                {
                    Ingredients = new List<string> { "Яйца", "Бекон", "Сыр" },
                    Recipes = new List<Recipe> {
                        new Recipe {
                            Title = "Завтрак разработчика",
                            Description = "Обжарьте бекон, добавьте яйца.",
                            MissingIngredients = new List<string> { "Зелень" }
                        }
                    }
                });
            }
            return Ok(result);
        }

        // НОВЫЙ МЕТОД: Сохранение рецепта в базу
        [HttpPost("save")]
        public async Task<IActionResult> SaveRecipe([FromBody] SavedRecipe recipe)
        {
            if (recipe == null) return BadRequest();

            _context.SavedRecipes.Add(recipe);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Рецепт успешно сохранен в твою базу!" });
        }
        
        [HttpDelete("saved/{id}")]
        public async Task<IActionResult> DeleteRecipe(int id)
        {
            var recipe = await _context.SavedRecipes.FindAsync(id);
            if (recipe == null) return NotFound();

            _context.SavedRecipes.Remove(recipe);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Рецепт удален!" });
        }
        // НОВЫЙ МЕТОД: Получение всех сохраненных рецептов
        [HttpGet("saved")]
        public IActionResult GetSavedRecipes()
        {
            var recipes = _context.SavedRecipes.ToList();
            return Ok(recipes);
        }
    }
}