using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseManager.DataAccessLayer
{
    public class TransactionFilter
    {
        public int page { get; set; } = 1;
        public DateTime? Date { get; set; } = null;
        public int? CategoryId { get; set; } = null;
        public string? Keyword { get; set; } = null;
    }
}
