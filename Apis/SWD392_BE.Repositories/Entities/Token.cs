using System;
using System.Collections.Generic;

namespace SWD392_BE.Repositories.Entities;

public partial class Token
{
    public string UserId { get; set; } = null!;

    public string? AccessToken { get; set; }

    public string? RefreshToken { get; set; }

    public DateTime? ExpiredTime { get; set; }

    public int Status { get; set; }
}
