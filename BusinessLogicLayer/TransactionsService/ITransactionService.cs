using ExpenseManager.BusinessLayer.TransactionsService.TransactionsDTO;
using ExpenseManager.DataAccessLayer;
using ExpenseManager.DataAccessLayer.Entities;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseManager.BusinessLayer.TransactionsService
{
    public interface ITransactionService
    {
        Task<TransactionPagingDTO> GetAllTransactionsAsync(TransactionFilter query);
        Task<TransactionDTO> GetTransactionByIdAsync(int transactionId);
        Task<bool> CreateTransactionAsync(CreateTransactionDTO newTransaction);
        Task<bool> UpdateTransactionAsync(UpdateTransactionDTO updatedTransaction);
        Task<bool> DeleteTransactionAsync(int transactionId);
        Task <decimal> GetSpendings(int days);
        Task<List<TopCategory>> GetTopSpendingCategories(TopSpendingsFilter filter);
        Task<IEnumerable<ChartDTO>> GetTransactionsChartData(string type);
        Task<IEnumerable<MonthlyReport>> GetMonthlyReport();
        Task<FileContentResult> GetTransactionsReportAsync(TransactionFilter query);
        Task CreateTransactionFromRecurring(int recurringId);
        Task<IEnumerable<TransactionUIDTO>> GetAllTransactionsAsync();
    }
}
