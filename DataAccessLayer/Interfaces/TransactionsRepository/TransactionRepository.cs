using ExpenseManager.DataAccessLayer.Data;
using ExpenseManager.DataAccessLayer.Entities;
using ExpenseManager.DataAccessLayer.Interfaces.GenericRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;


namespace ExpenseManager.DataAccessLayer.Interfaces.TransactionsRepository
{
    public class TransactionRepository : GenericRepo<Transaction>, ITransactionRepository
    {
        private readonly WalletManagerDbContext _context;

        public TransactionRepository(WalletManagerDbContext context):base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Task<int> GetAllTransactionsCountAsync()
        {
            return base.Count();
        }

        public async Task<IEnumerable<Transaction>> GetAllTransactionsAsync(int userId, TransactionFilter query)
        {
            var transactions = _context.Transactions
                .Include(t => t.Wallet)
                .Include(t => t.Category)
                .Include(c => c.CreateByNavigation)
                .Include(c => c.UpdateByNavigation)
                .Where(w => w.CreateBy == userId)
                .AsQueryable();

            if (!string.IsNullOrEmpty(query.Keyword))
            {
                transactions = transactions.Where(t => t.Note.Contains(query.Keyword) ||
                                                       t.Category.Name.Contains(query.Keyword));
            }
            if (query.DateFrom!= null || query.DateTo!= null)
            {
                transactions = transactions.Where(t => t.CreateDate.Date >= query.DateFrom && t.CreateDate.Date <= query.DateTo);
            }
            if (query.CategoryId != null)
            {
                transactions = transactions.Where(t => t.CategoryId == query.CategoryId);
            }
            if (query.WalletId != null)
            {
                transactions = transactions.Where(t => t.WalletId == query.WalletId);
            }

            return await transactions.ToListAsync();
        }

        public async Task<Transaction?> GetTransactionByIdAsync(int userId,int transactionId)
        {
            var transaction = await _context.Transactions
                .Include(c => c.CreateByNavigation)
                .Include(t => t.Wallet)
                .Include(t => t.Category)
                .Where(w => w.CreateBy == userId)
                .FirstOrDefaultAsync(c => c.Id == transactionId);
            return transaction;
        }

        public async Task<IEnumerable<Transaction>> GetSpendings(int userId, SpendingsFilter.Days days)
        {
            var transactions = _context.Transactions
                .Include(t => t.Wallet)
                .Include(t => t.Category)
                .Include(c => c.CreateByNavigation)
                .Include(c => c.UpdateByNavigation)
                .Where(w => w.CreateBy == userId && w.Category.Type.ToLower() == "expense")
                .AsQueryable();

            switch (days)
            {
                case SpendingsFilter.Days.Day:
                    transactions = transactions.Where(t => t.CreateDate.Day == DateTime.Now.Day);
                    break;
                case SpendingsFilter.Days.Month:
                    transactions = transactions.Where(t => t.CreateDate.Year == DateTime.Now.Year &&
                                                           t.CreateDate.Month == DateTime.Now.Month);
                    break;
                case SpendingsFilter.Days.Year:
                    transactions = transactions.Where(t => t.CreateDate.Year == DateTime.Now.Year);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(days), "Invalid value for days filter.");
            }

            return await transactions.ToListAsync();
        }

        public async Task<List<TopCategory>> GetTopSpendingCategoriesAsync(int userId, TopSpendingsFilter filter)
        {
            var date = DateTime.Now.AddDays(-filter.Days);

            var topCategories = await _context.Transactions
                .Where(t => t.CreateDate >= date &&
                            t.Category.Type.ToLower() == "expense"&&
                            t.CreateBy==userId)
                .GroupBy(t => new { t.CategoryId, t.Category.Name })
                .Select(g => new TopCategory
                {
                    CategoryName = g.Key.Name,
                    TotalSpent = g.Sum(t => t.Amount)
                })
                .OrderByDescending(c => c.TotalSpent)
                .Take(filter.TopN)
                .ToListAsync();

            return topCategories;
        }

        public async Task<List<object>> GetChartData(int userId, string type)
        {
            List<object> chartData = new List<object>();
            List<Transaction> transactions = (await GetAll(userId)).Where(t => t.Category.Type.ToLower() == type.ToLower()).ToList();
            
            if (transactions == null || !transactions.Any())
            {
                throw new InvalidOperationException("No transactions found for the user.");
            }

            List<string> labels = transactions
                .Select(t => t.CreateDate.Date)
                .Distinct()
                .OrderBy(d => d)
                .Select(d => d.ToString("yyyy-MM-dd")) 
                .ToList();

            List<decimal> total = transactions
                .GroupBy(t => t.CreateDate.Date)
                .OrderBy(g => g.Key)
                .Select(g => g.Sum(t => t.Amount))
                .ToList();

            chartData.Add(labels);
            chartData.Add(total);
            return chartData;
        }

        public async Task <List<Transaction>> GetAll(int userId)
        {
            return await _context.Transactions
                .Include(t => t.Wallet)
                .Include(t => t.Category)
                .Include(c => c.CreateByNavigation)
                .Include(c => c.UpdateByNavigation)
                .Where(c=> c.CreateBy == userId)
                .ToListAsync();
        }
    }
}
