using System;
using System.Collections.Generic;

namespace SWD392_BE.Repositories.Entities;

public partial class OrderDetail
{
    public int Id { get; set; }

    public string OrderId { get; set; } = null!;

    public string FoodId { get; set; } = null!;

    public int Status { get; set; }

    public int Price { get; set; }

    public int Quantity { get; set; }

    public string? Note { get; set; }

    public virtual Food Food { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;
}
