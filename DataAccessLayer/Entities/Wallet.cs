using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseManager.DataAccessLayer.Entities;
public partial class Wallet
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public decimal Balance { get; set; }

    public int CreateBy { get; set; }

    public DateTime CreateDate { get; set; }

    public int? UpdateBy { get; set; }

    public DateTime? UpdateDate { get; set; }

    public virtual User? CreateByNavigation { get; set; }

    public virtual User? UpdateByNavigation { get; set; }
    public virtual ICollection<Recurring> RecurringExpenses { get; set; } = new List<Recurring>();

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
