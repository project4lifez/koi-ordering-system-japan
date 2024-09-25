using System;
using System.Collections.Generic;

namespace KoiOrderingSystem.Models;

public partial class Koipackage
{
    public int PackageId { get; set; }

    public int? FarmId { get; set; }

    public int? VarietyId { get; set; }

    public int? KoiId { get; set; }

    public int? PoDetailId { get; set; }

    public string? PackageName { get; set; }

    public string? Description { get; set; }

    public decimal? Price { get; set; }

    public virtual KoiFarm? Farm { get; set; }

    public virtual KoiFish? Koi { get; set; }

    public virtual ICollection<KoiFarm> KoiFarms { get; set; } = new List<KoiFarm>();

    public virtual ICollection<KoiFish> KoiFishes { get; set; } = new List<KoiFish>();

    public virtual Podetail? PoDetail { get; set; }

    public virtual ICollection<Podetail> Podetails { get; set; } = new List<Podetail>();

    public virtual ICollection<Variety> Varieties { get; set; } = new List<Variety>();

    public virtual Variety? Variety { get; set; }
}
