using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseManager.BusinessLayer.WalletService.WalletDTO
{
    public class WalletUIDTO
    {
        public int Id { set; get; }
        public string Name { set; get; }
        public decimal Balance { get; set; }
    }
}
