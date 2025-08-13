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
        Task<IEnumerable<Transaction>> GetAllTransactionsAsync(int userId, TransactionFilter query);
        Task<Transaction?> GetTransactionByIdAsync(int userId, int transactionId);
        Task<IEnumerable<Transaction>> GetSpendings(int userId, SpendingsFilter.Days days);
        Task<List<TopCategory>> GetTopSpendingCategoriesAsync(int userId, TopSpendingsFilter filter);
        Task<List<object>> GetChartData(int userId, string type);
        Task<List<Transaction>> GetAll(int userId);

    }
}
