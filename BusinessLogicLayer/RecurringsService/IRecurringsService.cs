using ExpenseManager.BusinessLayer.RecurringsService.RecurringsDTO;
using ExpenseManager.DataAccessLayer.Entities;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseManager.BusinessLayer.RecurringsService
{
    public interface IRecurringsService
    {
        Task<RecurringPagingDTO> GetAllRecurringsAsync(int page = 1);
        Task<RecurringExpenseDTO> GetRecurringByIdAsync(int recurringId);
        Task<bool> CreateRecurringAsync(CreateRecurringDTO recurring);
        Task<bool> UpdateRecurringAsync(UpdateRecurringDTO recurring);
        Task<bool> DeleteRecurringAsync(int recurringId);
        Task<FileContentResult> GetRecurringsReportAsync();
        Task<IEnumerable<RecurringUIDTO>> GetAllRecurringsAsync();
    }
}
