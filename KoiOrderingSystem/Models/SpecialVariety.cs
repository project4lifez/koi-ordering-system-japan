using System;
using System.Collections.Generic;

namespace KoiOrderingSystem.Models;

public partial class SpecialVariety
{
    public int SpecialVarietyId { get; set; }

    public int? FarmId { get; set; }

    public int? VarietyId { get; set; }

    public string? SpecialDescription { get; set; }

    public virtual KoiFarm? Farm { get; set; }

    public virtual ICollection<KoiFarm> KoiFarms { get; set; } = new List<KoiFarm>();

    public virtual Variety? Variety { get; set; }
}
