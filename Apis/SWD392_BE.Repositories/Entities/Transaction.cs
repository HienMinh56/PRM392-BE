using System;
using System.Collections.Generic;

namespace SWD392_BE.Repositories.Entities;

public partial class Transaction
{
    public int Id { get; set; }

    public string TransactionId { get; set; } = null!;

    public string UserId { get; set; } = null!;

    public int Type { get; set; }

    public int Amount { get; set; }

    public int Status { get; set; }

    public TimeSpan? CreatTime { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public virtual ICollection<Order> Orders { get; } = new List<Order>();

    public virtual User User { get; set; } = null!;
}
