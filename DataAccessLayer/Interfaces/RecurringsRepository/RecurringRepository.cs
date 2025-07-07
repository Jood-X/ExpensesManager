using ExpenseManager.DataAccessLayer.Data;
using ExpenseManager.DataAccessLayer.Entities;
using ExpenseManager.DataAccessLayer.Interfaces.GenericRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseManager.DataAccessLayer.Interfaces.RecurringsRepository
{
    public class RecurringRepository: GenericRepo<Recurring>, IRecurringRepository
    {
        private readonly WalletManagerDbContext _context;

        public RecurringRepository(WalletManagerDbContext context): base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public Task<int> GetAllRecurringsCountAsync()
        {
            return base.Count();
        }

        public async Task<IEnumerable<Recurring>> GetAllRecurringsAsync()
        {
            var recurrings = _context.RecurringExpenses
                .Include(t => t.Wallet)
                .Include(t => t.Category)
                .Include(t => t.CreateByNavigation)
                .Include(t => t.UpdateByNavigation);
            return recurrings;
        }

        public async Task<Recurring?> GetRecurringByIdAsync(int recurringId)
        {
            var recurring = await _context.RecurringExpenses
                .Include(c => c.CreateByNavigation)
                .FirstOrDefaultAsync(c => c.Id == recurringId);
            return recurring;
        }
    }
}
