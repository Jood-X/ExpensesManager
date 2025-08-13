using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseManager.BusinessLayer.AuthorizationService.AuthDTO
{
    public class ConfirmCodeDTO
    {
        public string Email { get; set; }
        public string Code { get; set; }
    }
}
