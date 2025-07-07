namespace ExpenseManager.BusinessLayer.TransactionsService.TransactionsDTO
{
    public class TransactionPagingDTO
    {
        public List<TransactionDTO> Transactions { get; set; } = new List<TransactionDTO>();
        public int Pages { get; set; }
        public int CurrentPage { get; set; }
    }
}
