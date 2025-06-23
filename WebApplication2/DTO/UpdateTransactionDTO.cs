namespace WebApplication2.DTO
{
    public class UpdateTransactionDTO
    {
        public decimal? Amount { get; set; }

        public int? CategoryId { get; set; } 

        public int? WalletId { get; set; }

        public string? Note { get; set; }

        public int UpdateBy { get; set; }

        public DateTime? UpdateDate { get; set; }
    }
}
