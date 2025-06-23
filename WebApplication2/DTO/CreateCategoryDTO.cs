namespace WebApplication2.DTO
{
    public class CreateCategoryDTO
    {
        public string Name { get; set; } = null!;

        public int CreateBy { get; set; }

        public decimal Limit { get; set; }

        public string? Color { get; set; }

        public string Type { get; set; } = null!;
    }
}
