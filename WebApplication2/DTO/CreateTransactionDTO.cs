namespace WebApplication2.DTO
{
    public class CreateTransactionDTO
    {
        public decimal Amount { get; set; }

        public int CreateBy { get; set; }

        public int CategoryId { get; set; }

        public int WalletId { get; set; }

        public string? Note { get; set; }
    }
}
