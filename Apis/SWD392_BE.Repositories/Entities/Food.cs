using System;
using System.Collections.Generic;

namespace SWD392_BE.Repositories.Entities;

public partial class Food
{
    public int Id { get; set; }

    public string FoodId { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string StoreId { get; set; } = null!;

    public int Price { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public int Cate { get; set; }

    public string? Image { get; set; }

    public int Status { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime? DeletedDate { get; set; }

    public string? DeletedBy { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; } = new List<OrderDetail>();

    public virtual Store Store { get; set; } = null!;
}
