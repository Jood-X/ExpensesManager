using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseManager.BusinessLayer.TransactionsService.TransactionsDTO
{
    public class ChartDTO
    {
        public string Type { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
    }
}
