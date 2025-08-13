using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseManager.BusinessLayer.CategoriesService.CategoriesDTO
{
    public class CategoryUIDTO
    {
        public int Id { set; get; }
        public string Name { set; get; }
        public decimal Limit { get; set; }
        public string Type { get; set; }
    }
}
