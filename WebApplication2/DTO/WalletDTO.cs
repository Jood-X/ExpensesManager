using System.Text.Json.Serialization;
using WebApplication2.Models;

namespace WebApplication2.DTO
{
    public class WalletDTO
    {
        public string Name { get; set; } = null!;

        public decimal Balance { get; set; }

        public string CreatedBy { get; set; } = null!;

        public DateTime CreateDate { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? UpdatedBy { get; set; }
        
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public DateTime? UpdateDate { get; set; }
    }
}
