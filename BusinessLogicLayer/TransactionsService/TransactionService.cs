using AutoMapper;
using ExpenseManager.BusinessLayer.TransactionsService.TransactionsDTO;
using ExpenseManager.DataAccessLayer;
using ExpenseManager.DataAccessLayer.Entities;
using ExpenseManager.DataAccessLayer.Interfaces.CategoriesRepository;
using ExpenseManager.DataAccessLayer.Interfaces.RecurringsRepository;
using ExpenseManager.DataAccessLayer.Interfaces.TransactionsRepository;
using ExpenseManager.DataAccessLayer.Interfaces.WalletRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.Text;

namespace ExpenseManager.BusinessLayer.TransactionsService
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IWalletRepository _walletRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IRecurringRepository _recurringRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<TransactionService> _logger;

        public TransactionService(ITransactionRepository transactionRepository, IWalletRepository walletRepository, ICategoryRepository categoryRepository, IRecurringRepository recurringRepository, IMapper mapper, IConfiguration config, IHttpContextAccessor httpContextAccessor
            , ILogger<TransactionService> logger)
        {
            _transactionRepository = transactionRepository ?? throw new ArgumentNullException(nameof(TransactionRepository));
            _walletRepository = walletRepository ?? throw new ArgumentNullException(nameof(walletRepository));
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
            _recurringRepository = recurringRepository ?? throw new ArgumentNullException(nameof(recurringRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        private int GetUserIdOrThrow()
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("User ID is missing from token.");
            if (!int.TryParse(userId, out var userIdInt))
            {
                throw new InvalidOperationException("User ID is not in a valid format.");
            }
            return userIdInt;
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

            var userId = GetUserIdOrThrow();
            var transaction = await _transactionRepository.GetAllTransactionsAsync(userId, query);
            if (transaction == null)
            {
                throw new Exception("No transaction Found");
            }
            var totalCount = transaction.Count();
            var pageCount = (int)Math.Ceiling(totalCount / (double)pageResult);

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
            var transaction = await _transactionRepository.GetTransactionByIdAsync(userId, transactionId);

            if (transaction == null)
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
            transaction.CreateDate = DateTime.Now;
            transaction.CreateBy = userId;
            await _transactionRepository.Add(transaction);
            if(transaction.Category.Type.ToLower() == "expense")
            {
                wallet.Balance -= newTransaction.Amount;
            }
            else
            {
                wallet.Balance += newTransaction.Amount;
            }
            await _walletRepository.Update(wallet);
            return true;
        }

        public async Task<bool> UpdateTransactionAsync(UpdateTransactionDTO updatedTransaction)
        {
            var userId = GetUserIdOrThrow();

            var transaction = await _transactionRepository.GetTransactionByIdAsync(userId, updatedTransaction.Id);
            if (transaction == null)
            {
                throw new InvalidOperationException("Transaction not found or you do not have permission to update this transaction.");
            }
            decimal oldAmount = transaction.Amount;
            decimal newAmount = (decimal)updatedTransaction.Amount;
            _mapper.Map(updatedTransaction, transaction);
            transaction.UpdateDate = DateTime.Now;
            await _transactionRepository.Update(transaction);            
            if (transaction.WalletId != null && updatedTransaction.Amount != null)
            {
                var wallet = await _walletRepository.GetWalletByIdAsync(transaction.WalletId, userId);
                decimal difference = oldAmount - newAmount;
                if (transaction.Category.Type.ToLower() == "expense")
                {
                    wallet.Balance += difference;
                }
                else
                {
                    wallet.Balance -= difference;
                }
                await _walletRepository.Update(wallet);
            }
            return true;
        }

        public async Task<bool> DeleteTransactionAsync(int transactionId)
        {
            var userId = GetUserIdOrThrow();

            var transaction = await _transactionRepository.GetTransactionByIdAsync(userId, transactionId);
            if (transaction == null)
            {
                throw new InvalidOperationException("Transaction not found or you do not have permission to delete this transaction.");
            }
            await _transactionRepository.Delete(transaction);
            return true;
        }
                
        public async Task<decimal> GetSpendings(int days)
        {
            SpendingsFilter.Days day;
            if (days > 0 && days < 30)
            {
                day = SpendingsFilter.Days.Day;
            }
            else if (days >= 30 && days < 360)
            {
                day = SpendingsFilter.Days.Month;
            }
            else if (days >= 360)
            {
                day = SpendingsFilter.Days.Year;
            }
            else
            {
                throw new Exception("Number of days can't be negative");
            }

            var userId = GetUserIdOrThrow();
            var transaction = await _transactionRepository.GetSpendings(userId, day);
            if (transaction == null)
            {
                throw new InvalidOperationException("No transaction found.");
            }

            decimal totalSpendings = 0;
            foreach (var item in transaction)
            {
                totalSpendings += item.Amount;
            }
            return totalSpendings;
        }

        public async Task<List<TopCategory>> GetTopSpendingCategories(TopSpendingsFilter filter)
        {
            var userId = GetUserIdOrThrow();
            if(filter == null)
            {
                filter.Days = 30;
                filter.TopN = 3;
            }
            return await _transactionRepository.GetTopSpendingCategoriesAsync(userId, filter);
        }

        public async Task<IEnumerable<ChartDTO>> GetTransactionsChartData(string type)
        {
            var userId = GetUserIdOrThrow();
            var chartData = await _transactionRepository.GetChartData(userId, type);
            var labels = chartData[0] as List<string>;
            var data = chartData[1] as List<decimal>;

            if (labels == null || data == null || labels.Count != data.Count)
            {
                throw new InvalidOperationException("Chart data is not in the expected format.");
            }

            var chart = new List<ChartDTO>();
            for (int i = 0; i < labels.Count; i++)
            {
                var chartItem = new ChartDTO
                {
                    Label = labels[i],
                    TotalAmount = data[i]
                };
                chart.Add(chartItem);
            }
            return chart;
        }

        public async Task<IEnumerable<MonthlyReport>> GetMonthlyReport()
        {
            var userId = GetUserIdOrThrow();
            var transactions = await _transactionRepository.GetAll(userId);

            if (transactions == null)
            {
                throw new InvalidOperationException("No transactions found.");
            }
            var userTransactions = transactions
                .OrderBy(t => t.CreateDate.Month)
                .ToList();
                        
            var groupedByMonth = userTransactions
                .GroupBy(t => t.CreateDate.Month)
                .Select(g =>
                {
                    var monthExpenses = g.Where(t => t.Category.Type.ToLower() == "expense").Sum(t => t.Amount);
                    var monthIncomes = g.Where(t => t.Category.Type.ToLower() == "income").Sum(t => t.Amount);
                    return new MonthlyReport
                    {
                        Month = g.Key.ToString(),
                        TotalExpenses = monthExpenses,
                        TotalIncome = monthIncomes,
                        Balance = monthIncomes - monthExpenses
                    };
                });

            return groupedByMonth;
        }

        public async Task<FileContentResult> GetTransactionsReportAsync(TransactionFilter query)
        {
            var userId = GetUserIdOrThrow();
            string[] columnNames = { "Id", "Amount", "Category", "Wallet", "Note", "CreateBy", "CreateDate", "Amount", "UpdateDate" };
            var transactions = await _transactionRepository.GetAllTransactionsAsync(userId, query); 
            if (transactions == null || !transactions.Any())
            {
                throw new InvalidOperationException("No transaction found for the report.");
            }

            string csv = string.Empty;
            foreach (string columnName in columnNames)
            {
                csv += $"{columnName},";
            }
            csv += "\r\n";

            foreach (var transaction in transactions)
            {
                csv += $"{transaction.Id},{transaction.Amount},{transaction.Category.Name},{transaction.Wallet.Name}, {transaction.Note},{transaction.CreateByNavigation?.Name}, {transaction.CreateDate},{transaction.UpdateByNavigation?.Name},{transaction.UpdateDate}\r\n";
            }
            byte[] bytes = Encoding.ASCII.GetBytes(csv);
            return new FileContentResult(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                FileDownloadName = "TransactionsReport.csv"
            };
        }

        public async Task CreateTransactionFromRecurring(int recurringId)
        {
            var recurring = await _recurringRepository.GetRecurringByIdAsync(recurringId);
            if (recurring == null)
            {
                throw new InvalidOperationException("Recurring transaction not found or access denied.");
            }
            var newTransaction = new Transaction
            {
                CreateBy = recurring.CreateBy,
                CreateDate = DateTime.Now,
                Amount = recurring.Amount,
                CategoryId = recurring.CategoryId,
                WalletId = recurring.WalletId,
                Note = $"Recurring: {recurring.Id}",
            };
            await _transactionRepository.Add(newTransaction);
        }

        public async Task<IEnumerable<TransactionUIDTO>> GetAllTransactionsAsync()
        {
            var userId = GetUserIdOrThrow();
            var transactions = await _transactionRepository.GetAll(userId);
            var result = transactions.Select(transaction => _mapper.Map<TransactionUIDTO>(transaction)).ToList();
            return result;
        }
    }
}
