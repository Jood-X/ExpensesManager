using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpenseManager.DataAccessLayer.Entities;
using ExpenseManager.DataAccessLayer.Interfaces.GenericRepository;

namespace ExpenseManager.DataAccessLayer.Interfaces.TransactionsRepository
{
    public interface ITransactionRepository: IGenericRepo<Transaction>
    {
        Task<int> GetAllTransactionsCountAsync();
        Task<IEnumerable<Transaction>> GetAllTransactionsAsync(TransactionFilter query);
        Task<Transaction?> GetTransactionByIdAsync(int transactionId);
        Task<IEnumerable<Transaction>> GetSpendings(SpendingsFilter.Days days);
        Task<List<TopCategory>> GetTopSpendingCategoriesAsync(string userId, TopSpendingsFilter filter);
        Task<List<object>> GetChartData(int userId, string type);
    }
}
