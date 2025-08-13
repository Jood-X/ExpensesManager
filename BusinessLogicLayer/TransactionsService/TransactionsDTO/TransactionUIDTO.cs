using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseManager.BusinessLayer.TransactionsService.TransactionsDTO
{
    public class TransactionUIDTO
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreateDate { get; set; }
        public string? Note { get; set; }
        public int CategoryId { get; set; } 
        public int WalletId { get; set; }
        public string CategoryName { get; set; } = null!;
        public string WalletName { get; set; } = null!;
        public string CreatedBy { get; set; } = null!;
    }
}
