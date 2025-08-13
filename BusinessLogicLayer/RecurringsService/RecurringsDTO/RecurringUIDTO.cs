using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ExpenseManager.BusinessLayer.RecurringsService.RecurringsDTO
{
    public class RecurringUIDTO
    {
        public int Id { get; set; }
        public string CategoryName { get; set; } = null!;
        public string WalletName { get; set; } = null!;
        public string CreatedBy { get; set; } = null!;
        public decimal Amount { get; set; }
        public string RepeatInterval { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
