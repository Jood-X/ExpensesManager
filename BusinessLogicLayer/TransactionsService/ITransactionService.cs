using ExpenseManager.DataAccessLayer.Entities;
using ExpenseManager.BusinessLayer.TransactionsService.TransactionsDTO;
using ExpenseManager.DataAccessLayer;
using ExpenseManager.DataAccessLayer.Interfaces.TransactionsRepository;

namespace ExpenseManager.BusinessLayer.TransactionsService
{
    public interface ITransactionService
    {
        Task<TransactionPagingDTO> GetAllTransactionsAsync(TransactionFilter query);
        Task<TransactionDTO> GetTransactionByIdAsync(int transactionId);
        Task<bool> CreateTransactionAsync(CreateTransactionDTO newTransaction);
        Task<bool> UpdateTransactionAsync(int id, UpdateTransactionDTO updatedTransaction);
        Task<bool> DeleteTransactionAsync(int transactionId);
        Task <string> GetSpendings(int days);
        Task<List<TopCategory>> GetTopSpendingCategories(int days, int topN);
    }
}
