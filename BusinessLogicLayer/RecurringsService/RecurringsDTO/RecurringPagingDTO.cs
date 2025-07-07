namespace ExpenseManager.BusinessLayer.RecurringsService.RecurringsDTO
{
    public class RecurringPagingDTO
    {
        public List<RecurringExpenseDTO> Recurrings { get; set; } = new List<RecurringExpenseDTO>();
        public int Pages { get; set; }
        public int CurrentPage { get; set; }
    }
}
