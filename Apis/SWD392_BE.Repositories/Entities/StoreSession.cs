using System;
using System.Collections.Generic;

namespace SWD392_BE.Repositories.Entities;

public partial class StoreSession
{
    public int Id { get; set; }

    public string SessionId { get; set; } = null!;

    public string StoreId { get; set; } = null!;

    public virtual Session Session { get; set; } = null!;

    public virtual Store Store { get; set; } = null!;
}
