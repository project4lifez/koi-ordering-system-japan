﻿using System;
using System.Collections.Generic;

namespace KoiOrderingSystem.Models;

public partial class KoiFish
{
    public int KoiId { get; set; }

    public int? VarietyId { get; set; }

    public string? ImageUrl { get; set; }

    public string? KoiName { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<KoiFarm> KoiFarms { get; set; } = new List<KoiFarm>();

    public virtual ICollection<Podetail> Podetails { get; set; } = new List<Podetail>();

    public virtual Variety? Variety { get; set; }
}
