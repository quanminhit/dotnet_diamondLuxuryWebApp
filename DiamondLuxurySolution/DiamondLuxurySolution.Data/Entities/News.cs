﻿using System;
using System.Collections.Generic;

namespace DiamondLuxurySolution.Data.Entities;

public partial class News
{
    public int NewsId { get; set; }

    public string NewName { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string? Image { get; set; }

    public string? Description { get; set; }

    public DateTime? DateCreated { get; set; }

    public DateTime? DateModified { get; set; }

    public bool? IsOutstanding { get; set; }

    public Guid? Id { get; set; }

    public virtual AspNetUser? IdNavigation { get; set; }
}
