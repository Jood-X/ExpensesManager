namespace ExpenseManager.BusinessLayer.RecurringsService.RecurringsDTO
{
    public class CreateRecurringDTO
    {
        public decimal Amount { get; set; }

        public int CategoryId { get; set; }

        public int WalletId { get; set; }

        public string RepeatInterval { get; set; } = null!;

        public int IntervalValue { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}
