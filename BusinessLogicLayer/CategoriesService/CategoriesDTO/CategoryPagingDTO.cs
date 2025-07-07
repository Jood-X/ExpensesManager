namespace ExpenseManager.BusinessLayer.CategoriesService.CategoriesDTO
{
    public class CategoryPagingDTO
    {
        public List<CategoryDTO> Categories { get; set; } = new List<CategoryDTO>();
        public int Pages { get; set; }
        public int CurrentPage { get; set; }
    }
}
