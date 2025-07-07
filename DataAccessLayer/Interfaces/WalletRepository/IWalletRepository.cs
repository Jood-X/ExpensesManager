using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpenseManager.DataAccessLayer.Entities;
using ExpenseManager.DataAccessLayer.Interfaces.GenericRepository;

namespace ExpenseManager.DataAccessLayer.Interfaces.WalletRepository
{
    public interface IWalletRepository: IGenericRepo<Wallet>
    {
        Task<int> GetAllWalletsCountAsync(string userId);
        Task<IEnumerable<Wallet>> GetAllWalletsAsync(string userId, int pageResult, int page = 1);
        Task<Wallet?> GetWalletByIdAsync(int id, string userId);
    }
}
