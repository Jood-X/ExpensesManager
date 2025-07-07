using ExpenseManager.BusinessLayer.WalletService;
using ExpenseManager.BusinessLayer.WalletService.WalletDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ExpenseManager.DataAccessLayer.Entities;

namespace ExpenseManager.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class WalletsController : ControllerBase
    {
        private readonly IWalletService _walletService;
        public WalletsController(IWalletService walletService)
        {
            _walletService = walletService;
        }

        [HttpGet]
        public async Task<ApiResponse<WalletPagingDTO>> GetAll(string? searchTerm, int page=1)
        {
            try
            {
                var response = await _walletService.GetAllWalletsAsync(searchTerm, page);
                return ApiResponse<WalletPagingDTO>.SuccessResponse(response);
            }
            catch (Exception ex)
            {
                return ApiResponse<WalletPagingDTO>.ErrorResponse("An error occurred while retrieving users", ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ApiResponse<WalletsDTO>> GetByID(int id)
        {
            try
            {
                var wallet = await _walletService.GetWalletByIdAsync(id);
                return ApiResponse<WalletsDTO>.SuccessResponse(wallet);

            }
            catch (Exception ex)
            {
                return ApiResponse<WalletsDTO>.ErrorResponse("An error occurred while retrieving the user", ex.Message);
            }
        }

        [HttpPost]
        public async Task<ApiResponse<string>> Create(CreateWalletDTO newWallet)
        {
            try
            {
                await _walletService.CreateWalletAsync(newWallet);
                return ApiResponse<string>.SuccessResponse("Wallet Added Successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<string>.ErrorResponse("An error occurred while retrieving users", ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ApiResponse<string>> Update(int id, UpdateWalletDTO updatedWallet)
        {
            try
            {
                await _walletService.UpdateWalletAsync(updatedWallet);
                return ApiResponse<string>.SuccessResponse("Wallet Updated Successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<string>.ErrorResponse("An error occurred while updating the wallet", ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ApiResponse<string>> Delete(int id)
        {
            try
            {
                await _walletService.DeleteWalletAsync(id);
                return ApiResponse<string>.SuccessResponse("Wallet Deleted Successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<string>.ErrorResponse("An error occurred while deleting the wallet", ex.Message);
            }          
        }
    }
}
