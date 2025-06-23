using System.Text.Json.Serialization;
using WebApplication2.Models;

namespace WebApplication2.DTO
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
