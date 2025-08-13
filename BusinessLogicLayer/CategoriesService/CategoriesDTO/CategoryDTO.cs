using System.Text.Json.Serialization;

namespace ExpenseManager.BusinessLayer.CategoriesService.CategoriesDTO
{
    public class CategoryDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public string CreatedBy { get; set; } = null!;

        public DateTime CreateDate { get; set; }

        public decimal Limit { get; set; }

        public string? Color { get; set; }

        public string Type { get; set; } = null!;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? UpdatedBy { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public DateTime? UpdateDate { get; set; }
    }
}
