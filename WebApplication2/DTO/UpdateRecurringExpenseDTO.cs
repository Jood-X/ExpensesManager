namespace WebApplication2.DTO
{
    public class UpdateRecurringExpenseDTO
    {
        public decimal? Amount { get; set; }

        public int? CategoryId { get; set; }

        public int? WalletId { get; set; }

        public string? RepeatInterval { get; set; } = null!;

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int UpdateBy { get; set; }

        public DateTime? UpdateDate { get; set; }
    }
}
