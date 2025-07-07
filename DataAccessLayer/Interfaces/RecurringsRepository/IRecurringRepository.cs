using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpenseManager.DataAccessLayer.Entities;
using ExpenseManager.DataAccessLayer.Interfaces.GenericRepository;

namespace ExpenseManager.DataAccessLayer.Interfaces.RecurringsRepository
{
    public interface IRecurringRepository: IGenericRepo<Recurring>
    {
        Task<int> GetAllRecurringsCountAsync();
        Task<IEnumerable<Recurring>> GetAllRecurringsAsync();
        Task<Recurring?> GetRecurringByIdAsync(int recurringId);
    }
}
