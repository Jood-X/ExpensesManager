namespace ExpenseManager.BusinessLayer.RecurringsService.RecurringsDTO
{
    public class UpdateRecurringDTO
    {
        public int Id { get; set; }
        public decimal? Amount { get; set; }

        public int? CategoryId { get; set; }

        public int? WalletId { get; set; }

        public string? RepeatInterval { get; set; } = null!;

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}
