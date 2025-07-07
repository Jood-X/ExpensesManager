using ExpenseManager.DataAccessLayer.Data;
using ExpenseManager.DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpenseManager.DataAccessLayer.Interfaces.GenericRepository;

namespace ExpenseManager.DataAccessLayer.Interfaces.WalletRepository
{
    public class WalletRepository : GenericRepo<Wallet>, IWalletRepository
    {
        private readonly WalletManagerDbContext _context;

        public WalletRepository(WalletManagerDbContext context) : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<int> GetAllWalletsCountAsync(string userId)
        {
            var wallets = _context.Wallets
                .Where(w => w.CreateBy.ToString() == userId)
                .Count();
            return wallets;
        }
        public async Task<IEnumerable<Wallet>> GetAllWalletsAsync(string userId, int pageResult, int page = 1)
        {
            var wallets = _context.Wallets
                .Include(c => c.CreateByNavigation)
                .Include(c => c.UpdateByNavigation)
                .Where(r => r.CreateBy.ToString() == userId);
            return wallets;
        }
        public async Task<Wallet?> GetWalletByIdAsync(int id, string userId)
        {
            var wallet = await _context.Wallets
                .Include(w => w.CreateByNavigation)
                .Include(w => w.UpdateByNavigation)
                .FirstOrDefaultAsync(w => w.Id == id && w.CreateBy.ToString() == userId);
            return wallet;
        }
    }
}
