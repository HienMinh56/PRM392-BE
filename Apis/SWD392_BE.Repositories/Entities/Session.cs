using System;
using System.Collections.Generic;

namespace SWD392_BE.Repositories.Entities;

public partial class Session
{
    public int Id { get; set; }

    public string SessionId { get; set; } = null!;

    public TimeSpan StartTime { get; set; }

    public TimeSpan EndTime { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime? DeletedDate { get; set; }

    public string? DeletedBy { get; set; }

    public virtual ICollection<Order> Orders { get; } = new List<Order>();

    public virtual ICollection<StoreSession> StoreSessions { get; } = new List<StoreSession>();
}
