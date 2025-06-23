using WebApplication2.Models;

namespace WebApplication2.DTO
{
    public class UpdateWalletDTO
    {
        public string? Name { get; set; } = null!;

        public decimal? Balance { get; set; }

        public int UpdateBy { get; set; }

        public DateTime? UpdateDate { get; set; }
    }
}
