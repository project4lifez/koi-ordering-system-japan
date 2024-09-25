using System;
using System.Collections.Generic;

namespace KoiOrderingSystem.Models;

public partial class KoiFish
{
    public int KoiId { get; set; }

    public string? Type { get; set; }

    public int? FarmId { get; set; }

    public decimal? Price { get; set; }

    public string? Size { get; set; }

    public int? VarietyId { get; set; }

    public int? PoDetailId { get; set; }

    public int? PackageId { get; set; }

    public virtual KoiFarm? Farm { get; set; }

    public virtual ICollection<KoiFarm> KoiFarms { get; set; } = new List<KoiFarm>();

    public virtual ICollection<Koipackage> Koipackages { get; set; } = new List<Koipackage>();

    public virtual Koipackage? Package { get; set; }

    public virtual Podetail? PoDetail { get; set; }

    public virtual ICollection<Podetail> Podetails { get; set; } = new List<Podetail>();

    public virtual ICollection<Variety> Varieties { get; set; } = new List<Variety>();

    public virtual Variety? Variety { get; set; }
}
