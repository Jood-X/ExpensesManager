using ExpenseManager.DataAccessLayer.Entities;
using ExpenseManager.BusinessLayer.TransactionsService.TransactionsDTO;
using ExpenseManager.DataAccessLayer;

namespace ExpenseManager.BusinessLayer.TransactionsService
{
    public interface ITransactionService
    {
        Task<TransactionPagingDTO> GetAllTransactionsAsync(TransactionFilter query);
        Task<TransactionDTO> GetTransactionByIdAsync(int transactionId);
        Task<bool> CreateTransactionAsync(CreateTransactionDTO newTransaction);
        Task<bool> UpdateTransactionAsync(int id, UpdateTransactionDTO updatedTransaction);
        Task<bool> DeleteTransactionAsync(int transactionId);
        Task<IEnumerable<Transaction>> GetTransactionsByWalletIdAsync(int walletId);
        Task <decimal> GetSpendings(int interval, int day, int month, int year); 
        Task<decimal> GetDayTransactions();
        Task<decimal> GetMonthTransactions();
        Task<decimal> GetYearTransactions();

        Task<string> GetTopSpendingCategory();
    }
}
