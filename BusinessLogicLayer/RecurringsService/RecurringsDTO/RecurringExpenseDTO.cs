using System.Text.Json.Serialization;

namespace ExpenseManager.BusinessLayer.RecurringsService.RecurringsDTO
{
    public class RecurringExpenseDTO
    {
        public string CategoryName { get; set; } = null!;
        public string WalletName { get; set; } = null!;
        public string CreatedBy { get; set; } = null!;

        public decimal Amount { get; set; }

        public string RepeatInterval { get; set; } = null!;

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public DateTime CreateDate { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? UpdatedBy { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public DateTime? UpdateDate { get; set; }
    }
}
