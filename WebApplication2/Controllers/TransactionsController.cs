using ExpenseManager.BusinessLayer.TransactionsService;
using ExpenseManager.BusinessLayer.TransactionsService.TransactionsDTO;
using ExpenseManager.DataAccessLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ExpenseManager.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionsController(ITransactionService transactionService)
        {
            _transactionService = transactionService ?? throw new ArgumentNullException(nameof(transactionService));
        }

        [HttpGet]
        public async Task<ApiResponse<TransactionPagingDTO>> GetAll([FromQuery] TransactionFilter query)
        {
            try
            {
                var response = await _transactionService.GetAllTransactionsAsync(query);
                return ApiResponse<TransactionPagingDTO>.SuccessResponse(response);
            }
            catch (Exception ex)
            {
                return ApiResponse<TransactionPagingDTO>.ErrorResponse("An error occurred while retrieving transactions", ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ApiResponse<TransactionDTO>> GetByID(int id)
        {
            try
            {
                var transaction = await _transactionService.GetTransactionByIdAsync(id);
                return ApiResponse<TransactionDTO>.SuccessResponse(transaction);
            }
            catch (Exception ex)
            {
                return ApiResponse<TransactionDTO>.ErrorResponse("An error occurred while retrieving the transaction", ex.Message);
            }
        }

        [HttpPost]
        public async Task<ApiResponse<string>> Create(CreateTransactionDTO newTrans)
        {
            try
            {
                await _transactionService.CreateTransactionAsync(newTrans);
                return ApiResponse<string>.SuccessResponse("Transaction Added Successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<string>.ErrorResponse("An error occurred while creating the transaction", ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ApiResponse<string>> Update(int id, UpdateTransactionDTO updatedTrans)
        {
            try
            {
                var result = await _transactionService.UpdateTransactionAsync(id, updatedTrans);
                return ApiResponse<string>.SuccessResponse("Transaction Updated Successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<string>.ErrorResponse("An error occurred while updating the transaction", ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ApiResponse<string>> Delete(int id)
        {
            try
            {
                var result = await _transactionService.DeleteTransactionAsync(id);
                return ApiResponse<string>.SuccessResponse("Transaction Deleted Successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<string>.ErrorResponse("An error occurred while deleting the transaction", ex.Message);
            }
        }

        [HttpGet("TotalSpendings/{days}")]
        public async Task<ApiResponse<string>> GetTotalSpendings(int days)
        {
            try
            {
                var totalSpendings = await _transactionService.GetSpendings(days);
                return ApiResponse<string>.SuccessResponse(totalSpendings);
            }
            catch (Exception ex)
            {
                return ApiResponse<string>.ErrorResponse("An error occurred while retrieving total spendings", ex.Message);
            }
        }

        [HttpGet("TopCategories")]
        public async Task<ApiResponse<List<TopCategory>>> GetTopSpendingCategories([FromQuery] TopSpendingsFilter filter)
        {
            try
            {
                var result = await _transactionService.GetTopSpendingCategories(filter);
                return ApiResponse<List<TopCategory>>.SuccessResponse(result);
            }
            catch (Exception ex)
            {
                return ApiResponse<List<TopCategory>>.ErrorResponse("Failed to retrieve top categories", ex.Message);
            }
        }

        [HttpGet("ChartData")]
        public async Task<ApiResponse<IEnumerable<ChartDTO>>> GetTransactionsChartData(string type = "expense/income")
        {
            try
            {
                var chartData = await _transactionService.GetTransactionsChartData(type);
                return ApiResponse<IEnumerable<ChartDTO>>.SuccessResponse(chartData);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<ChartDTO>>.ErrorResponse("An error occurred while retrieving chart data", ex.Message);
            }
        }

    }
}
