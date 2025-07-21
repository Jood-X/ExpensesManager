using ExpenseManager.BusinessLayer.WalletService.WalletDTO;
using ExpenseManager.DataAccessLayer.Entities;
using Microsoft.AspNetCore.Mvc;
using System;

namespace ExpenseManager.BusinessLayer.WalletService
{
    public interface IWalletService
    {
        Task<WalletPagingDTO> GetAllWalletsAsync(string? searchTerm, int page = 1);
        Task<IEnumerable<WalletUIDTO>> GetAllWalletsAsync();
        Task<WalletsDTO> GetWalletByIdAsync(int walletId);
        Task<bool> CreateWalletAsync(CreateWalletDTO newWallet);
        Task<bool> UpdateWalletAsync(UpdateWalletDTO updateWallet);
        Task<bool> DeleteWalletAsync(int walletId);
        Task<FileContentResult> GetWalletsReportAsync();
    }
}
