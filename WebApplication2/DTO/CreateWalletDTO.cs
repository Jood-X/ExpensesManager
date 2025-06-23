using WebApplication2.Models;

namespace WebApplication2.DTO
{
    public class CreateWalletDTO
    {
        public string Name { get; set; } = null!;

        public decimal Balance { get; set; }

        public int CreateBy { get; set; }

    }
}
