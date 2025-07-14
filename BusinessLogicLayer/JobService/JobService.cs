using Hangfire;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseManager.BusinessLayer.JobService
{
    public class JobService : IJobService
    {
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly IRecurringJobManager _recurringJobManager;

        public JobService(IBackgroundJobClient backgroundJobClient, IRecurringJobManager recurringJobManager)
        {
            _backgroundJobClient = backgroundJobClient ?? throw new ArgumentNullException(nameof(backgroundJobClient));
            _recurringJobManager = recurringJobManager ?? throw new ArgumentNullException(nameof(recurringJobManager));
        }

        public void FireAndForgetJob()
        {
            _backgroundJobClient.Enqueue(() => Console.WriteLine("Background Job Triggered"));
        }

        public void RecurringJob()
        {
            var options = new RecurringJobOptions
            {
                TimeZone = TimeZoneInfo.Local
            };

            _recurringJobManager.AddOrUpdate("jobId", () => Console.WriteLine(), Cron.Hourly, options);
        }

        public void ContinuationJob()
        {
            var jobId = _backgroundJobClient.Enqueue(() => Console.WriteLine());
            _backgroundJobClient.ContinueJobWith(jobId, () => Console.WriteLine());
        }

        public void DelayedJob()
        {
            _backgroundJobClient.Schedule(() => Console.WriteLine(), TimeSpan.FromSeconds(60));
        }
        public enum IntervalUnit { Minutes, Hour, Day, Week, Month }

        public void ScheduleRecurringJob(int id, String interval, int intervalValue, DateTime startDate, DateTime? endDate)
        {
            if (intervalValue <= 0)
                throw new ArgumentOutOfRangeException(nameof(intervalValue));

            if (endDate is not null && startDate >= endDate)
                throw new ArgumentException("Start date must be earlier than end date.");
            
            var intervalUnit = Enum.Parse<IntervalUnit>(interval, true);

            var cron = intervalUnit switch
            {
                IntervalUnit.Minutes => $"*/{intervalValue} * * * *",
                IntervalUnit.Hour => $"0 */{intervalValue} * * *",
                IntervalUnit.Day => $"0 0 */{intervalValue} * *",
                IntervalUnit.Week => $"0 0 * * {((int)startDate.DayOfWeek)}",
                IntervalUnit.Month => $"0 0 {startDate.Day} */{intervalValue} *",
                _ => throw new NotSupportedException()
            };

            var options = new RecurringJobOptions { TimeZone = TimeZoneInfo.Local };

            _recurringJobManager.AddOrUpdate<RecurringExpenseJob>(
                $"recurring-expense-{id}",
                job => job.Handle(id,startDate, endDate),
                cron,
                options);
        }
    }
}
