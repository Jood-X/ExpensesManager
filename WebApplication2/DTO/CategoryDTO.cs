using System.Text.Json.Serialization;
using WebApplication2.Models;

namespace WebApplication2.DTO
{
    public class CategoryDTO
    {
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
