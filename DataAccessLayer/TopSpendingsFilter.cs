using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseManager.DataAccessLayer
{
    public class TopSpendingsFilter
    {
        public int Days { get; set; }
        public int TopN { get; set; }

    }
}
