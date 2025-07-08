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

        public async Task<IEnumerable<Transaction>> GetAllTransactionsAsync(TransactionFilter query)
        {
            var transactions = _context.Transactions
                .Include(t => t.Wallet)
                .Include(t => t.Category)
                .Include(c => c.CreateByNavigation)
                .Include(c => c.UpdateByNavigation)
                .AsQueryable();

            if (!string.IsNullOrEmpty(query.Keyword))
            {
                transactions = transactions.Where(t => t.Note.Contains(query.Keyword) ||
                                                       t.Category.Name.Contains(query.Keyword));
            }
            if (query.Date!= null)
            {
                transactions = transactions.Where(t => t.CreateDate.Date == query.Date);
            }
            if (query.CategoryId != null)
            {
                transactions = transactions.Where(t => t.CategoryId == query.CategoryId);
            }

            return await transactions.ToListAsync();
        }

        public async Task<Transaction?> GetTransactionByIdAsync(int transactionId)
        {
            var transaction = await _context.Transactions
                .Include(c => c.CreateByNavigation)
                .FirstOrDefaultAsync(c => c.Id == transactionId);
            return transaction;
        }

        public async Task<IEnumerable<Transaction>> GetSpendings(int days)
        {
            var transactions = _context.Transactions
                    .Include(t => t.Wallet)
                    .Include(t => t.Category)
                    .Include(c => c.CreateByNavigation)
                    .Include(c => c.UpdateByNavigation)
                    .AsQueryable();

            switch (days)
            {
                case 1:
                    transactions = transactions.Where(t => t.CreateDate.Date.Day == DateTime.Now.Day);
                    break;
                case 7:
                    transactions = transactions.Where(t => t.CreateDate.Date.Month == DateTime.Now.Month);
                    break;
                case 30:
                    transactions = transactions.Where(t => t.CreateDate.Date.Year == DateTime.Now.Year);
                    break;
                default:
                    transactions = transactions.Where(t => t.CreateDate.Date.Day == DateTime.Now.Day);
                    break;
            }
            return await transactions.ToListAsync();
        }

        public async Task<List<TopCategory>> GetTopSpendingCategoriesAsync(string userId, int days, int topN)
        {
            var date = DateTime.UtcNow.AddDays(-days);

            var topCategories = await _context.Transactions
                .Where(t => t.CreateDate >= date &&
                            t.Category.Type.ToLower() == "expense")
                .GroupBy(t => new { t.CategoryId, t.Category.Name })
                .Select(g => new TopCategory
                {
                    CategoryName = g.Key.Name,
                    TotalSpent = g.Sum(t => t.Amount)
                })
                .OrderByDescending(c => c.TotalSpent)
                .Take(topN)
                .ToListAsync();

            return topCategories;
        }



    }
}
