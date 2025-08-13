using ExpenseManager.BusinessLayer.CategoriesService.CategoriesDTO;
using ExpenseManager.BusinessLayer.RecurringsService;
using ExpenseManager.BusinessLayer.RecurringsService.RecurringsDTO;
using ExpenseManager.DataAccessLayer.Entities;
using ExpenseManager.DataAccessLayer.Interfaces.GenericRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace ExpenseManager.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RecurringsController : ControllerBase
    {
        private IGenericRepo<Recurring> genericRepo;
        private readonly IRecurringsService _recurringsService;
        private readonly ILogger<RecurringsController> _logger;

        public RecurringsController(IRecurringsService recurringsService, IGenericRepo<Recurring> repo, ILogger<RecurringsController> logger)
        {
            _recurringsService = recurringsService ?? throw new ArgumentNullException(nameof(recurringsService));
            genericRepo = repo ?? throw new ArgumentNullException(nameof(repo));
            _logger = logger;
            _logger.LogDebug("Nlog is integrated to RecurringsController");
        }

        [HttpGet]
        public async Task<ApiResponse<RecurringPagingDTO>> GetAll(int page = 1)
        {
            try
            {
                var response = await _recurringsService.GetAllRecurringsAsync(page);
                return ApiResponse<RecurringPagingDTO>.SuccessResponse(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return ApiResponse<RecurringPagingDTO>.ErrorResponse("An error occurred while retrieving recurrings", ex.Message);
            }
        }

        [HttpGet("MyRecurring")]
        public async Task<ApiResponse<IEnumerable<RecurringUIDTO>>> GetAll()
        {
            try
            {
                var response = await _recurringsService.GetAllRecurringsAsync();
                return ApiResponse<IEnumerable<RecurringUIDTO>>.SuccessResponse(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return ApiResponse<IEnumerable<RecurringUIDTO>>.ErrorResponse("An error occurred while retrieving recurrings", ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ApiResponse<RecurringExpenseDTO>> GetByID(int id)
        {
            try
            {
                var recurring = await _recurringsService.GetRecurringByIdAsync(id);
                return ApiResponse<RecurringExpenseDTO>.SuccessResponse(recurring);

            }
            catch (Exception ex)
            {
                return ApiResponse<RecurringExpenseDTO>.ErrorResponse("An error occurred while retrieving the recurring", ex.Message);
            }
        }

        [HttpPost]
        public async Task<ApiResponse<string>> Create(CreateRecurringDTO newRecurring)
        {
            try
            {
                await _recurringsService.CreateRecurringAsync(newRecurring);
                return ApiResponse<string>.SuccessResponse("Recurring Added Successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return ApiResponse<string>.ErrorResponse("An error occurred while creating the recurring", ex.Message);
            }
        }

        [HttpPut]
        public async Task<ApiResponse<string>> Update(UpdateRecurringDTO updatedRecurring)
        {
            try
            {
                await _recurringsService.UpdateRecurringAsync(updatedRecurring);
                return ApiResponse<string>.SuccessResponse("Recurring Updated Successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return ApiResponse<string>.ErrorResponse("An error occurred while updating the recurring", ex.Message);
            }
        }
        
        [HttpDelete("{id}")]
        public async Task<ApiResponse<string>> DeleteRecurring(int id)
        {
            try
            {
                await _recurringsService.DeleteRecurringAsync(id);
                return ApiResponse<string>.SuccessResponse("Recurring Deleted Successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return ApiResponse<string>.ErrorResponse("An error occurred while deleting the recurring", ex.Message);
            }
        }

        [HttpGet("report")]
        public async Task<IActionResult> GetCategoriesReport()
        {
            try
            {
                var report = await _recurringsService.GetRecurringsReportAsync();
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
