using AutoMapper;
using ExpenseManager.BusinessLayer.CategoriesService.CategoriesDTO;
using ExpenseManager.BusinessLayer.JobService;
using ExpenseManager.BusinessLayer.RecurringsService.RecurringsDTO;
using ExpenseManager.DataAccessLayer.Entities;
using ExpenseManager.DataAccessLayer.Interfaces.CategoriesRepository;
using ExpenseManager.DataAccessLayer.Interfaces.RecurringsRepository;
using ExpenseManager.DataAccessLayer.Interfaces.TransactionsRepository;
using ExpenseManager.DataAccessLayer.Interfaces.WalletRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using System.Text;

namespace ExpenseManager.BusinessLayer.RecurringsService
{
    public class RecurringsService : IRecurringsService
    {
        private readonly IRecurringRepository _recurringRepository;
        private readonly IWalletRepository _walletRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IJobService _jobService;

        public RecurringsService(IRecurringRepository recurringRepository, IWalletRepository walletRepository, ICategoryRepository categoryRepository, IMapper mapper, IConfiguration config, IHttpContextAccessor httpContextAccessor, IJobService jobService)
        {
            _recurringRepository = recurringRepository ?? throw new ArgumentNullException(nameof(TransactionRepository));
            _walletRepository = walletRepository ?? throw new ArgumentNullException(nameof(walletRepository));
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _httpContextAccessor = httpContextAccessor;
            _jobService = jobService ?? throw new ArgumentNullException(nameof(jobService));
        }
        private string GetUserIdOrThrow()
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("User ID is missing from token.");
            return userId;
        }

        public async Task<RecurringPagingDTO> GetAllRecurringsAsync(int page = 1)
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
            var allRecurrings = await _recurringRepository.GetAllRecurringsAsync();

            var recurrings = allRecurrings
                .Where(w => w.CreateBy.ToString() == userId);

            var totalCount = recurrings.Count();
            var pageCount = (int)Math.Ceiling(totalCount / (double)pageResult);

            recurrings = recurrings
                .Skip((page - 1) * pageResult)
                .Take(pageResult);

            var response = new RecurringPagingDTO
            {
                Recurrings = recurrings.Select(t => _mapper.Map<RecurringExpenseDTO>(t)).ToList(),
                Pages = pageCount,
                CurrentPage = page
            };

            return response;
        }

        public async Task<RecurringExpenseDTO> GetRecurringByIdAsync(int recurringId)
        {
            var userId = GetUserIdOrThrow();
            var recurring = await _recurringRepository.GetRecurringByIdAsync(recurringId);

            if (recurring == null || recurring.CreateBy.ToString() != userId)
            {
                throw new InvalidOperationException("Recurring not found.");
            }

            return _mapper.Map<RecurringExpenseDTO>(recurring);
        }
        public async Task<bool> CreateRecurringAsync(CreateRecurringDTO newRecurring)
        {
            var userId = GetUserIdOrThrow();

            if (newRecurring == null)
            {
                throw new ArgumentNullException(nameof(newRecurring), "Recurring cannot be null");
            }
            var wallet = await _walletRepository.GetWalletByIdAsync(newRecurring.WalletId, int.Parse(userId));
            if (wallet == null)
            {
                throw new InvalidOperationException("Wallet not found or access denied.");
            }

            var category = await _categoryRepository.GetCategoryByIdAsync(newRecurring.CategoryId);
            if (category == null)
            {
                throw new InvalidOperationException("Category not found or access denied.");
            }

            if (newRecurring.Amount > wallet.Balance)
            {
                throw new InvalidOperationException("Insufficient wallet balance.");
            }

            if (category.Limit > 0 && newRecurring.Amount > category.Limit)
            {
                throw new InvalidOperationException("Recurring amount exceeds category limit.");
            }
            var intervalValue = newRecurring.IntervalValue;
            var recurring = _mapper.Map<Recurring>(newRecurring);
            recurring.CreateDate = DateTime.Now;
            recurring.CreateBy = int.Parse(userId);
            await _recurringRepository.Add(recurring);

            
            _jobService.ScheduleRecurringJob(recurring.Id, recurring.RepeatInterval, intervalValue, recurring.StartDate, recurring.EndDate);
            

            return true;
        }

        public async Task<bool> UpdateRecurringAsync(UpdateRecurringDTO updateRecurring)
        {
            var userId = GetUserIdOrThrow();

            var recurring = await _recurringRepository.GetRecurringByIdAsync(updateRecurring.Id);
            if (recurring == null || recurring.CreateBy.ToString() != userId)
            {
                throw new InvalidOperationException("Recurring not found or you do not have permission to update this recurring.");
            }
            _mapper.Map(updateRecurring, recurring);
            recurring.UpdateDate = DateTime.Now;
            await _recurringRepository.Update(recurring);
            return true;
        }

        public async Task<bool> DeleteRecurringAsync(int recurringId)
        {
            var userId = GetUserIdOrThrow();

            var recurring = await _recurringRepository.GetRecurringByIdAsync(recurringId);
            if (recurring == null || recurring.CreateBy.ToString() != userId)
            {
                throw new InvalidOperationException("Recurring not found or you do not have permission to delete this recurring.");
            }
            await _recurringRepository.Delete(recurring);
            return true;
        }

        public async Task<FileContentResult> GetRecurringsReportAsync()
        {
            string[] columnNames = { "Id", "Amount", "Wallet", "Category", "RepeatInterval", "StartDate", "EndDate", "CreateBy", "CreateDate", "UpdateBy", "UpdateDate" };
            var recurrings = await _recurringRepository.GetAllRecurringsAsync();
            if (recurrings == null || !recurrings.Any())
            {
                throw new InvalidOperationException("No recurrings found for the report.");
            }

            string csv = string.Empty;
            foreach (string columnName in columnNames)
            {
                csv += $"{columnName},";
            }
            csv += "\r\n";

            foreach (var recurring in recurrings)
            {
                csv += $"{recurring.Id},{recurring.Amount},{recurring.Wallet.Name}, {recurring.Category.Name}, {recurring.RepeatInterval}, {recurring.StartDate}, {recurring.EndDate},{recurring.CreateByNavigation?.Name}, {recurring.CreateDate},{recurring.UpdateByNavigation?.Name},{recurring.UpdateDate}\r\n";
            }
            byte[] bytes = Encoding.ASCII.GetBytes(csv);
            return new FileContentResult(bytes, "text/csv")
            {
                FileDownloadName = "RecurringsReport.csv"
            };
        }

        public async Task<IEnumerable<RecurringUIDTO>> GetAllRecurringsAsync()
        {
            var userId = GetUserIdOrThrow();
            var allRecurrings = await _recurringRepository.GetAll();
            var recurrings = allRecurrings
                .Where(w => w.CreateBy.ToString() == userId);
            var result = recurrings.Select(recurring => _mapper.Map<RecurringUIDTO>(recurring)).ToList();
            return result;
        }
    }
}
