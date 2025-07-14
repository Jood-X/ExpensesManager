using ExpenseManager.BusinessLayer.TransactionsService;
using ExpenseManager.BusinessLayer.TransactionsService.TransactionsDTO;
using ExpenseManager.DataAccessLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using WebApplication2.Controllers;

namespace ExpenseManager.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        private readonly ILogger<TransactionsController> _logger;

        public TransactionsController(ITransactionService transactionService, ILogger<TransactionsController> logger)
        {
            _transactionService = transactionService ?? throw new ArgumentNullException(nameof(transactionService));
            _logger = logger;
            _logger.LogDebug("Nlog is integrated to TransactionsController");
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
                _logger.LogError(ex.Message);
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
                _logger.LogError(ex.Message);
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
                _logger.LogError(ex.Message);
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
                _logger.LogError(ex.Message);
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
                _logger.LogError(ex.Message);
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
                _logger.LogError(ex.Message);
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
                _logger.LogError(ex.Message);
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
                _logger.LogError(ex.Message);
                return ApiResponse< IEnumerable < ChartDTO >>.ErrorResponse("An error occurred while retrieving total spendings", ex.Message);
            }
        }

        [HttpGet("MonthlyReport")]
        public async Task<ApiResponse<IEnumerable<MonthlyReport>>> GetMonthlyReport()
        {
            try
            {
                var chartData = await _transactionService.GetMonthlyReport();
                return ApiResponse<IEnumerable<MonthlyReport>>.SuccessResponse(chartData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return ApiResponse<IEnumerable<MonthlyReport>>.ErrorResponse("An error occurred while retrieving monthly report", ex.Message);
            }
        }

        [HttpGet("report")]
        public async Task<IActionResult> GetTransactionsReport()
        {
            try
            {
                var report = await _transactionService.GetTransactionsReportAsync();
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
