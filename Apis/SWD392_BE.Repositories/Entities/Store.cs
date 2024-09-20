using System;
using System.Collections.Generic;

namespace SWD392_BE.Repositories.Entities;

public partial class Store
{
    public int Id { get; set; }

    public string StoreId { get; set; } = null!;

    public string AreaId { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Address { get; set; } = null!;

    public int Status { get; set; }

    public string? Phone { get; set; }

    public TimeSpan OpenTime { get; set; }

    public TimeSpan CloseTime { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime? DeletedDate { get; set; }

    public string? DeletedBy { get; set; }

    public virtual Area Area { get; set; } = null!;

    public virtual ICollection<Food> Foods { get; } = new List<Food>();

    public virtual ICollection<Order> Orders { get; } = new List<Order>();

    public virtual ICollection<StoreSession> StoreSessions { get; } = new List<StoreSession>();
}
