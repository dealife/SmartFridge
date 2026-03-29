using System.ComponentModel.DataAnnotations;

namespace SmartFridgeAPI.Models
{
    public class SavedRecipe
    {
        [Key] // Это уникальный номер (ID) рецепта в базе
        public int Id { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime SavedAt { get; set; } = DateTime.Now; // Дата сохранения
    }
}