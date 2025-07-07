using ExpenseManager.DataAccessLayer.Entities;
using ExpenseManager.BusinessLayer.RecurringsService.RecurringsDTO;

namespace ExpenseManager.BusinessLayer.RecurringsService
{
    public interface IRecurringsService
    {
        Task<RecurringPagingDTO> GetAllRecurringsAsync(int page = 1);
        Task<RecurringExpenseDTO> GetRecurringByIdAsync(int recurringId);
        Task<bool> CreateRecurringAsync(CreateRecurringDTO recurring);
        Task<bool> UpdateRecurringAsync(int id, UpdateRecurringDTO recurring);
        Task<bool> DeleteRecurringAsync(int recurringId);
        Task<IEnumerable<Recurring>> GetRecurringsByWalletIdAsync(int walletId);
    }
}
