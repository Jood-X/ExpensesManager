namespace WebApplication2.DTO
{
    public class UpdateCategoryDTO
    {
        public string? Name { get; set; } = null!;

        public decimal? Limit { get; set; }

        public string? Color { get; set; }

        public string? Type { get; set; } = null!;
    }
}
