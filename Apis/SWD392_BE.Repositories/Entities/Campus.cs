using System;
using System.Collections.Generic;

namespace SWD392_BE.Repositories.Entities;

public partial class Campus
{
    public int Id { get; set; }

    public string CampusId { get; set; } = null!;

    public string AreaId { get; set; } = null!;

    public string Name { get; set; } = null!;

    public int Status { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime? DeletedDate { get; set; }

    public string? DeletedBy { get; set; }

    public virtual Area Area { get; set; } = null!;

    public virtual ICollection<User> Users { get; } = new List<User>();
}
