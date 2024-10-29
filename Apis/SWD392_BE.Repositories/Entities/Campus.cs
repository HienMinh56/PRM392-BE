﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace SWD392_BE.Repositories.Entities;

public partial class Campus
{
    public int Id { get; set; }

    public string CampusId { get; set; }

    public string AreaId { get; set; }

    public string Name { get; set; }

    public int Status { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string CreatedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string ModifiedBy { get; set; }

    public DateTime? DeletedDate { get; set; }

    public string DeletedBy { get; set; }

    public virtual Area Area { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}