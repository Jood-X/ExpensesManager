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
        Task<int> GetAllWalletsCountAsync(int userId);
        Task<IEnumerable<Wallet>> GetAllWalletsAsync(int userId);
        Task<Wallet?> GetWalletByIdAsync(int id, int userId);
    }
}
