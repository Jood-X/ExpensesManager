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
        public DateTime? DateFrom { get; set; } = null;
        public DateTime? DateTo { get; set; } = null;
        public int? CategoryId { get; set; } = null;
        public int? WalletId { get; set; } = null;
        public string? Keyword { get; set; } = null;


    }
}
