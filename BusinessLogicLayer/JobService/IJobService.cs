using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ExpenseManager.BusinessLayer.JobService.JobService;

namespace ExpenseManager.BusinessLayer.JobService
{
    public interface IJobService
    {
        void FireAndForgetJob();
        void RecurringJob();
        void DelayedJob();
        void ContinuationJob();
        void ScheduleRecurringJob(int id, string interval, int intervalValue, DateTime startDate, DateTime? endDate);
    }
}
