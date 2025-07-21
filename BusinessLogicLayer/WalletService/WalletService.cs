using AutoMapper;
using ExpenseManager.BusinessLayer.WalletService.WalletDTO;
using ExpenseManager.DataAccessLayer.Entities;
using ExpenseManager.DataAccessLayer.Interfaces.WalletRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using System.Text;

namespace ExpenseManager.BusinessLayer.WalletService
{
    public class WalletService : IWalletService
    {
        private readonly IWalletRepository _walletRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public WalletService(IWalletRepository walletRepository, IMapper mapper, IConfiguration config, IHttpContextAccessor httpContextAccessor)
        {
            _walletRepository = walletRepository;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _httpContextAccessor = httpContextAccessor;
        }
        private string GetUserIdOrThrow()
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value; // Use IHttpContextAccessor to access the user
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("User ID is missing from token.");
            return userId;
        }

        public async Task<WalletPagingDTO> GetAllWalletsAsync(string? searchTerm, int page = 1)
        {
            var pageSizeSetting = _config["AppSettings:PageSize"];
            if (string.IsNullOrEmpty(pageSizeSetting))
            {
                throw new InvalidOperationException("Page size configuration is missing.");
            }

            if (!int.TryParse(pageSizeSetting, out var pageResult))
            {
                throw new InvalidOperationException("Page size configuration is invalid.");
            }
            var userId = GetUserIdOrThrow();
            var totalWalletsCount = await _walletRepository.GetAllWalletsCountAsync(userId);
            var pageCount = (int)Math.Ceiling(totalWalletsCount / (double)pageResult);

            var allWallets = await _walletRepository.GetAllWalletsAsync(userId, page);
            var wallets = allWallets
                .Where(w => w.CreateBy.ToString() == userId);

            if (searchTerm != null)
            {
                searchTerm = searchTerm.ToLower();
                wallets = wallets.Where(u => u.Name.ToLower().Contains(searchTerm));
            }
            wallets = wallets
                .Skip((page - 1) * pageResult)
                .Take(pageResult);

            var response = new WalletPagingDTO
            {
                Wallets = wallets.Select(wallet => _mapper.Map<WalletsDTO>(wallet)).ToList(),
                Pages = pageCount,
                CurrentPage = page
            };
            return response;
        }

        public async Task<WalletsDTO> GetWalletByIdAsync(int walletId)
        {
            var userId = GetUserIdOrThrow();
            var wallet = await _walletRepository.GetWalletByIdAsync(walletId, userId);

            if (wallet == null || wallet.CreateBy.ToString() != userId)
            {
                throw new InvalidOperationException("Wallet not found.");
            }

            return _mapper.Map<WalletsDTO>(wallet);
        }
        public async Task<bool> CreateWalletAsync(CreateWalletDTO newWallet)
        {
            var userId = GetUserIdOrThrow();
            if (newWallet == null)
            {
                throw new ArgumentNullException("Wallet cannot be null");
            }
            var wallet = _mapper.Map<Wallet>(newWallet);
            wallet.CreateDate = DateTime.Now;
            wallet.CreateBy = int.Parse(userId);
            await _walletRepository.Add(wallet);
            return true;
        }

        public async Task<bool> UpdateWalletAsync(UpdateWalletDTO updateWallet)
        {
            var userId = GetUserIdOrThrow();
            var wallet = await _walletRepository.GetWalletByIdAsync(updateWallet.Id, userId);
            if (wallet == null || wallet.CreateBy.ToString() != userId)
            {
                throw new InvalidOperationException("Wallet not found or you do not have permission to update this wallet.");
            }
            _mapper.Map(updateWallet, wallet);
            wallet.UpdateDate = DateTime.Now;
            await _walletRepository.Update(wallet);
            return true;
        }

        public async Task<bool> DeleteWalletAsync(int walletId)
        {
            var userId = GetUserIdOrThrow();
            var wallet = await _walletRepository.GetWalletByIdAsync(walletId, userId);
            if (wallet == null || wallet.CreateBy.ToString() != userId)
            {
                throw new InvalidOperationException("Wallet not found or you do not have permission to delete this wallet.");
            }
            await _walletRepository.Delete(wallet);
            return true;
        }

        public async Task<FileContentResult> GetWalletsReportAsync()
        {
            string[] columnNames = { "Id", "Name", "Balance", "CreateBy", "CreateDate", "UpdateBy", "UpdateDate"};
            var wallets = await _walletRepository.GetAll();
            if (wallets == null || !wallets.Any())
            {
                throw new InvalidOperationException("No wallets found for the report.");
            }

            string csv = string.Empty;
            foreach (string columnName in columnNames)
            {
                csv += $"{columnName},";
            }
            csv += "\r\n";

            foreach (var wallet in wallets)
            {
                csv += $"{wallet.Id},{wallet.Name},{wallet.Balance}, {wallet.CreateByNavigation?.Name},{wallet.CreateDate},{wallet.UpdateByNavigation?.Name},{wallet.UpdateDate}\r\n";
            }
            byte[] bytes = Encoding.ASCII.GetBytes(csv);
            return new FileContentResult(bytes, "text/csv")
            {
                FileDownloadName = "WalletsReport.csv"
            };
        }

        public async Task<IEnumerable<WalletUIDTO>> GetAllWalletsAsync()
        {
            var userId = GetUserIdOrThrow();
            var allWallets = await _walletRepository.GetAll();
            var wallets = allWallets
                .Where(w => w.CreateBy.ToString() == userId);
            var result = wallets.Select(wallet => _mapper.Map<WalletUIDTO>(wallet)).ToList();
            return result;
        }
    }
}
