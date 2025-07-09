using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseManager.BusinessLayer.TransactionsService.TransactionsDTO
{
    public class MonthlyReport
    {
        public string Month { get; set; } = null!;
        public decimal TotalExpenses { get; set; }
        public decimal TotalIncome { get; set; }
        public decimal Balance { get; set; }
    }
}
