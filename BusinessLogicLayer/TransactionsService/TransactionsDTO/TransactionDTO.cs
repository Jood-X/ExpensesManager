using System.Text.Json.Serialization;

namespace ExpenseManager.BusinessLayer.TransactionsService.TransactionsDTO
{
    public class TransactionDTO
    {
        public decimal Amount { get; set; }
        public DateTime CreateDate { get; set; }
        public string? Note { get; set; }
        public string CategoryName { get; set; } = null!;
        public string WalletName { get; set; } = null!;
        public string CreatedBy { get; set; } = null!;
        
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? UpdatedBy { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public DateTime? UpdateDate { get; set; }

    }
}
