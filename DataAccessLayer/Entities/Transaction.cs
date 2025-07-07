using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseManager.DataAccessLayer.Entities;
public partial class Transaction
{
    public int Id { get; set; }

    public int CategoryId { get; set; }

    public int CreateBy { get; set; }

    public int WalletId { get; set; }

    public decimal Amount { get; set; }

    public DateTime CreateDate { get; set; }

    public int? UpdateBy { get; set; }

    public DateTime? UpdateDate { get; set; }

    public string? Note { get; set; }

    public virtual Category? Category { get; set; }

    public virtual User? CreateByNavigation { get; set; }

    public virtual User? UpdateByNavigation { get; set; }

    public virtual Wallet? Wallet { get; set; }
}
