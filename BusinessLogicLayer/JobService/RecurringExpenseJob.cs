using ExpenseManager.BusinessLayer.TransactionsService;
using Hangfire;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseManager.BusinessLayer.JobService
{
    public class RecurringExpenseJob
    {
        private readonly ITransactionService _transaction;
        private readonly IRecurringJobManager _recurring;

        public RecurringExpenseJob(ITransactionService transaction,
                                   IRecurringJobManager recurring)
        {
            _transaction = transaction;
            _recurring = recurring;
        }

        public async Task Handle(int recurringId,DateTime startDate, DateTime? endDate) 
        {
            if (endDate is not null && DateTime.Now > endDate.Value)
            {
                _recurring.RemoveIfExists($"recurring-expense-{recurringId}");
                return;
            }

            if( startDate <= DateTime.Now && endDate >= DateTime.Now)
            {
                await _transaction.CreateTransactionFromRecurring(recurringId);
            }
        }
    }

}
