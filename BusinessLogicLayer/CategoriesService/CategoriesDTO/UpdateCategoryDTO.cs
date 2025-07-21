namespace ExpenseManager.BusinessLayer.CategoriesService.CategoriesDTO
{
    public class UpdateCategoryDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; } = null!;

        public decimal? Limit { get; set; }

        public string? Color { get; set; }

        public string? Type { get; set; } = null!;
    }
}
