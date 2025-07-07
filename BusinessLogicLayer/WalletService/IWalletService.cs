using System;
using ExpenseManager.DataAccessLayer.Entities;
using ExpenseManager.BusinessLayer.WalletService.WalletDTO;

namespace ExpenseManager.BusinessLayer.WalletService
{
    public interface IWalletService
    {
        Task<WalletPagingDTO> GetAllWalletsAsync(string? searchTerm, int page = 1);
        Task<WalletsDTO> GetWalletByIdAsync(int walletId);
        Task<bool> CreateWalletAsync(CreateWalletDTO newWallet);
        Task<bool> UpdateWalletAsync(UpdateWalletDTO updateWallet);
        Task<bool> DeleteWalletAsync(int walletId);
    }
}
