using System;
using System.Collections.Generic;

namespace SWD392_BE.Repositories.Entities;

public partial class User
{
    public int Id { get; set; }

    public string UserId { get; set; } = null!;

    public string? Name { get; set; }

    public string UserName { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string CampusId { get; set; } = null!;

    public string? Phone { get; set; }

    public int Role { get; set; }

    public int Balance { get; set; }

    public int Status { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime? DeletedDate { get; set; }

    public string? DeletedBy { get; set; }

    public virtual Campus Campus { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; } = new List<Order>();

    public virtual ICollection<Transaction> Transactions { get; } = new List<Transaction>();
}
