namespace ExpenseManager.BusinessLayer.TransactionsService.TransactionsDTO
{
    public class UpdateTransactionDTO
    {
        public int Id { get; set; }
        public decimal? Amount { get; set; } 

        public int? CategoryId { get; set; } 

        public int? WalletId { get; set; } 

        public string? Note { get; set; } 

    }
}
