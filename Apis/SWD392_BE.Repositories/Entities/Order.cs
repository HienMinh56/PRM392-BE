using System;
using System.Collections.Generic;

namespace SWD392_BE.Repositories.Entities;

public partial class Order
{
    public int Id { get; set; }

    public string OrderId { get; set; } = null!;

    public string UserId { get; set; } = null!;

    public string SessionId { get; set; } = null!;

    public int Price { get; set; }

    public int Quantity { get; set; }

    public string StoreId { get; set; } = null!;

    public string TransactionId { get; set; } = null!;

    public int Status { get; set; }

    public TimeSpan CreatedTime { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string? ModifiedBy { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; } = new List<OrderDetail>();

    public virtual Session Session { get; set; } = null!;

    public virtual Store Store { get; set; } = null!;

    public virtual Transaction Transaction { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
