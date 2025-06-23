using WebApplication2.Models;

namespace WebApplication2.DTO
{
    public class CreateRecurringExpenseDTO
    {
        public decimal Amount { get; set; }

        public int CreateBy { get; set; }

        public int CategoryId { get; set; }

        public int WalletId { get; set; }

        public string RepeatInterval { get; set; } = null!;

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}
