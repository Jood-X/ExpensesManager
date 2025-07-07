using AutoMapper;
using ExpenseManager.BusinessLayer.RecurringsService.RecurringsDTO;
using ExpenseManager.BusinessLayer.TransactionsService.TransactionsDTO;
using ExpenseManager.DataAccessLayer.Entities;
using ExpenseManager.DataAccessLayer.Interfaces.CategoriesRepository;
using ExpenseManager.DataAccessLayer.Interfaces.RecurringsRepository;
using ExpenseManager.DataAccessLayer.Interfaces.TransactionsRepository;
using ExpenseManager.DataAccessLayer.Interfaces.WalletRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

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
        private DateTime date = DateTime.UtcNow;

        public RecurringsService(IRecurringRepository recurringRepository, IWalletRepository walletRepository, ICategoryRepository categoryRepository, IMapper mapper, IConfiguration config, IHttpContextAccessor httpContextAccessor)
        {
            _recurringRepository = recurringRepository ?? throw new ArgumentNullException(nameof(TransactionRepository));
            _walletRepository = walletRepository ?? throw new ArgumentNullException(nameof(walletRepository));
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _httpContextAccessor = httpContextAccessor;
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
            var totalUsersCount = await _recurringRepository.GetAllRecurringsCountAsync();
            var pageCount = (int)Math.Ceiling(totalUsersCount / (double)pageResult);

            var userId = GetUserIdOrThrow();
            var allRecurrings = await _recurringRepository.GetAllRecurringsAsync();

            var recurrings = allRecurrings
                .Where(w => w.CreateBy.ToString() == userId);

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
            var wallet = await _walletRepository.GetWalletByIdAsync(newRecurring.WalletId, userId);
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
            var recurring = _mapper.Map<Recurring>(newRecurring);
            recurring.CreateDate = date;
            recurring.CreateBy = int.Parse(userId);
            await _recurringRepository.Add(recurring);
            return true;
        }

        public async Task<bool> UpdateRecurringAsync(int id, UpdateRecurringDTO updateRecurring)
        {
            var userId = GetUserIdOrThrow();

            var recurring = await _recurringRepository.GetRecurringByIdAsync(id);
            if (recurring == null || recurring.CreateBy.ToString() != userId)
            {
                throw new InvalidOperationException("Recurring not found or you do not have permission to update this recurring.");
            }
            recurring.UpdateDate = date;
            _mapper.Map(updateRecurring, recurring);
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

        public Task<IEnumerable<Recurring>> GetRecurringsByWalletIdAsync(int walletId)
        {
            throw new NotImplementedException();
        }        
    }
}
