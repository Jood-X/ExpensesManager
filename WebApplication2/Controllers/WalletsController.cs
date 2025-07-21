using ExpenseManager.BusinessLayer.WalletService;
using ExpenseManager.BusinessLayer.WalletService.WalletDTO;
using ExpenseManager.DataAccessLayer.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Claims;
using WebApplication2.Controllers;

namespace ExpenseManager.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class WalletsController : ControllerBase
    {
        private readonly IWalletService _walletService;
        private readonly ILogger<WalletsController> _logger;

        public WalletsController(IWalletService walletService, ILogger<WalletsController> logger)
        {
            _walletService = walletService;
            _logger = logger;
            _logger.LogDebug("Nlog is integrated to WalletsController");
        }

        [HttpGet]
        public async Task<ApiResponse<WalletPagingDTO>> GetAllPaging(string? searchTerm, int page=1)
        {
            try
            {
                var response = await _walletService.GetAllWalletsAsync(searchTerm, page);
                return ApiResponse<WalletPagingDTO>.SuccessResponse(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return ApiResponse<WalletPagingDTO>.ErrorResponse("An error occurred while retrieving wallets", ex.Message);
            }
        }

        [HttpGet("MyWallets")]
        public async Task<ApiResponse<IEnumerable<WalletUIDTO>>> GetAll()
        {
            try
            {
                var response = await _walletService.GetAllWalletsAsync();
                return ApiResponse<IEnumerable<WalletUIDTO>>.SuccessResponse(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return ApiResponse<IEnumerable<WalletUIDTO>>.ErrorResponse("An error occurred while retrieving wallets", ex.Message);
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
                _logger.LogError(ex.Message);
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
                _logger.LogError(ex.Message);
                return ApiResponse<string>.ErrorResponse("An error occurred while retrieving users", ex.Message);
            }
        }

        [HttpPut]
        public async Task<ApiResponse<string>> Update(UpdateWalletDTO updatedWallet)
        {
            try
            {
                await _walletService.UpdateWalletAsync(updatedWallet);
                return ApiResponse<string>.SuccessResponse("Wallet Updated Successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
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
                _logger.LogError(ex.Message);
                return ApiResponse<string>.ErrorResponse("An error occurred while deleting the wallet", ex.Message);
            }          
        }

        [HttpGet("report")]
        public async Task<IActionResult> GetWalletsReport()
        {
            try
            {
                var report = await _walletService.GetWalletsReportAsync();
                return File(report.FileContents, report.ContentType, report.FileDownloadName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(new ApiResponse<string>(false, "An error occurred while generating the report", null, ex.Message));
            }
        }
    }
}
