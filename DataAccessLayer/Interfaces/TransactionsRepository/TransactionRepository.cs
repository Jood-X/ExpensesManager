using ExpenseManager.DataAccessLayer.Data;
using ExpenseManager.DataAccessLayer.Entities;
using ExpenseManager.DataAccessLayer.Interfaces.GenericRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
