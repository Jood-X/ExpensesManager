using AutoMapper;
using ExpenseManager.BusinessLayer.TransactionsService.TransactionsDTO;
using ExpenseManager.DataAccessLayer;
using ExpenseManager.DataAccessLayer.Entities;
using ExpenseManager.DataAccessLayer.Interfaces.CategoriesRepository;
using ExpenseManager.DataAccessLayer.Interfaces.TransactionsRepository;
using ExpenseManager.DataAccessLayer.Interfaces.WalletRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;

namespace ExpenseManager.BusinessLayer.TransactionsService
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IWalletRepository _walletRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private DateTime date = DateTime.UtcNow;

        public TransactionService(ITransactionRepository transactionRepository,IWalletRepository walletRepository, ICategoryRepository categoryRepository , IMapper mapper, IConfiguration config, IHttpContextAccessor httpContextAccessor)
        {
            _transactionRepository = transactionRepository ?? throw new ArgumentNullException(nameof(TransactionRepository));
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

        public async Task<TransactionPagingDTO> GetAllTransactionsAsync(TransactionFilter query)
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
            var totalUsersCount = await _transactionRepository.GetAllTransactionsCountAsync();
            var pageCount = (int)Math.Ceiling(totalUsersCount / (double)pageResult);

            var userId = GetUserIdOrThrow();
            var allTransactions = await _transactionRepository.GetAllTransactionsAsync(query);

            var transaction = allTransactions
                .Where(w => w.CreateBy.ToString() == userId);

            transaction = transaction
                .Skip((query.page - 1) * pageResult)
                .Take(pageResult);

            var response = new TransactionPagingDTO
            {
                Transactions = transaction.Select(t => _mapper.Map<TransactionDTO>(t)).ToList(),
                Pages = pageCount,
                CurrentPage = query.page
            };

            return response;
        }

        public async Task<TransactionDTO> GetTransactionByIdAsync(int transactionId)
        {
            var userId = GetUserIdOrThrow();
            var transaction = await _transactionRepository.GetTransactionByIdAsync(transactionId);

            if (transaction == null || transaction.CreateBy.ToString() != userId)
            {
                throw new InvalidOperationException("Transaction not found.");
            }

            return _mapper.Map<TransactionDTO>(transaction);
        }

        public async Task<bool> CreateTransactionAsync(CreateTransactionDTO newTransaction)
        {
            var userId = GetUserIdOrThrow();

            if (newTransaction == null)
            {
                throw new ArgumentNullException(nameof(newTransaction), "Transaction cannot be null");
            }
            var wallet = await _walletRepository.GetWalletByIdAsync(newTransaction.WalletId, userId);
            if (wallet == null)
            {
                throw new InvalidOperationException("Wallet not found or access denied.");
            }

            var category = await _categoryRepository.GetCategoryByIdAsync(newTransaction.CategoryId);
            if (category == null)
            {
                throw new InvalidOperationException("Category not found or access denied.");
            }

            if (newTransaction.Amount > wallet.Balance)
            {
                throw new InvalidOperationException("Insufficient wallet balance.");
            }

            if (category.Limit > 0 && newTransaction.Amount > category.Limit)
            {
                throw new InvalidOperationException("Transaction amount exceeds category limit.");
            }

            var transaction = _mapper.Map<Transaction>(newTransaction);
            transaction.CreateDate = date;
            transaction.CreateBy = int.Parse(userId);
            await _transactionRepository.Add(transaction);
            wallet.Balance -= newTransaction.Amount;
            await _walletRepository.Update(wallet);
            return true;
        }
        public async Task<bool> UpdateTransactionAsync(int id,UpdateTransactionDTO updatedTransaction)
        {
            var userId = GetUserIdOrThrow();

            var transaction = await _transactionRepository.GetTransactionByIdAsync(id);
            if (transaction == null || transaction.CreateBy.ToString() != userId)
            {
                throw new InvalidOperationException("Transaction not found or you do not have permission to update this transaction.");
            }
            transaction.UpdateDate = date;
            _mapper.Map(updatedTransaction, transaction);
            await _transactionRepository.Update(transaction);
            return true;
        }

        public async Task<bool> DeleteTransactionAsync(int transactionId)
        {
            var userId = GetUserIdOrThrow();

            var transaction = await _transactionRepository.GetTransactionByIdAsync(transactionId);
            if (transaction == null || transaction.CreateBy.ToString() != userId)
            {
                throw new InvalidOperationException("Transaction not found or you do not have permission to delete this transaction.");
            }
            await _transactionRepository.Delete(transaction);
            return true;
        }

        public async Task<string> GetSpendings(int days)
        {
            var userId = GetUserIdOrThrow();
            var allTransactions = await _transactionRepository.GetSpendings(days);
            var transaction = allTransactions
                .Where(w => w.CreateBy.ToString() == userId);
            
            if (transaction == null)
            {
                throw new InvalidOperationException("No transaction found.");
            }

            decimal totalSpendings = 0;
            foreach (var item in transaction)
            {
                totalSpendings += item.Amount;
            }
            return $"Total spent for {days} days is: {totalSpendings}";
        }

        public async Task<List<TopCategory>> GetTopSpendingCategories(int days, int topN)
        {
            var userId = GetUserIdOrThrow();
            return await _transactionRepository.GetTopSpendingCategoriesAsync(userId, days, topN);
        }
    }
}
