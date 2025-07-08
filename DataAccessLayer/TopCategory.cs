using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseManager.DataAccessLayer
{
    public class TopCategory
    {
        public string CategoryName { get; set; } = null!;
        public decimal TotalSpent { get; set; }
    }
}
