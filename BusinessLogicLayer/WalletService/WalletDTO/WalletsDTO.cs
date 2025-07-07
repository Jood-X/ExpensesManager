using ExpenseManager.DataAccessLayer.Entities;
using System.Text.Json.Serialization;

namespace ExpenseManager.BusinessLayer.WalletService.WalletDTO
{
    public class WalletsDTO
    {
        public string Name { get; set; } = null!;

        public decimal Balance { get; set; }

        public string CreatedBy { get; set; } = null!;

        public DateTime CreateDate { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? UpdatedBy { get; set; }
        
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public DateTime? UpdateDate { get; set; }

        public static implicit operator WalletsDTO?(Wallet? v)
        {
            throw new NotImplementedException();
        }
    }
}
