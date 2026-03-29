using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
 // Убедись, что этот Namespace совпадает с твоим AppDbContext
using SmartFridgeAPI.Data;   // Чтобы видел AppDbContext
using SmartFridgeAPI.Models;
namespace SmartFridge.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FridgeController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;
        private readonly AppDbContext _context; // 1. Добавили поле для базы данных

        // 2. Добавили AppDbContext в конструктор
        public FridgeController(IConfiguration config, HttpClient httpClient, AppDbContext context)
        {
            _config = config;
            _httpClient = httpClient;
            _context = context;
        }

        // ПОЛУЧЕНИЕ СОХРАНЕННЫХ РЕЦЕПТОВ
        [HttpGet("saved")]
        public async Task<IActionResult> GetSavedRecipes()
        {
            var recipes = await _context.Recipes.OrderByDescending(r => r.Id).ToListAsync();
            return Ok(recipes);
        }

        // СОХРАНЕНИЕ НОВОГО РЕЦЕПТА
        [HttpPost("save")]
        public async Task<IActionResult> SaveRecipe([FromBody] Recipe recipe)
        {
            if (recipe == null) return BadRequest();

            recipe.SavedAt = DateTime.Now; // Добавляем дату сохранения
            _context.Recipes.Add(recipe);
            await _context.SaveChangesAsync();
            return Ok();
        }

        // УДАЛЕНИЕ РЕЦЕПТА
        [HttpDelete("saved/{id}")]
        public async Task<IActionResult> DeleteRecipe(int id)
        {
            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe == null) return NotFound();

            _context.Recipes.Remove(recipe);
            await _context.SaveChangesAsync();
            return Ok();
        }

        // АНАЛИЗ ФОТО ЧЕРЕЗ GEMINI
        [HttpPost("analyze")]
        public async Task<IActionResult> Analyze(IFormFile photo)
        {
            if (photo == null || photo.Length == 0) return BadRequest("Фото не получено");

            var apiKey = _config["GeminiApiKey"];
            if (string.IsNullOrEmpty(apiKey)) return StatusCode(500, "API Ключ не найден");

            var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent?key={apiKey}";

            using var ms = new MemoryStream();
            await photo.CopyToAsync(ms);
            var base64Image = Convert.ToBase64String(ms.ToArray());

            var requestBody = new
            {
                contents = new[] {
                    new {
                        parts = new object[] {
                            new { text = "Ты — шеф-повар. Проанализируй фото холодильника. Перечисли продукты и предложи 2-3 рецепта. Ответ верни СТРОГО в формате JSON: { \"ingredients\": [\"продукт1\"], \"recipes\": [{ \"title\": \"название\", \"description\": \"описание\" }] }" },
                            new { inline_data = new { mime_type = "image/jpeg", data = base64Image } }
                        }
                    }
                }
            };

            try
            {
                var response = await _httpClient.PostAsJsonAsync(url, requestBody);
                var jsonResponse = await response.Content.ReadAsStringAsync();

                using var doc = JsonDocument.Parse(jsonResponse);
                var textResponse = doc.RootElement.GetProperty("candidates")[0]
                                      .GetProperty("content")
                                      .GetProperty("parts")[0]
                                      .GetProperty("text").GetString();

                var cleanJson = textResponse.Replace("```json", "").Replace("```", "").Trim();
                return Content(cleanJson, "application/json");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка ИИ: {ex.Message}");
            }
        }
    }
}