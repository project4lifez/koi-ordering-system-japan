using System;
using System.Collections.Generic;

namespace KoiOrderingSystem.Models;

public partial class KoiFarm
{
    public int FarmId { get; set; }

    public int? TripDetailId { get; set; }

    public int? PoId { get; set; }

    public int? SpecialVarietyId { get; set; }

    public int? KoiId { get; set; }

    public int? PackageId { get; set; }

    public string? FarmName { get; set; }

    public string? Location { get; set; }

    public string? ContactInfo { get; set; }

    public virtual KoiFish? Koi { get; set; }

    public virtual ICollection<KoiFish> KoiFishes { get; set; } = new List<KoiFish>();

    public virtual ICollection<Koipackage> Koipackages { get; set; } = new List<Koipackage>();

    public virtual Koipackage? Package { get; set; }

    public virtual Po? Po { get; set; }

    public virtual ICollection<Podetail> Podetails { get; set; } = new List<Podetail>();

    public virtual ICollection<SpecialVariety> SpecialVarieties { get; set; } = new List<SpecialVariety>();

    public virtual SpecialVariety? SpecialVariety { get; set; }

    public virtual TripDetail? TripDetail { get; set; }

    public virtual ICollection<TripDetail> TripDetails { get; set; } = new List<TripDetail>();
}
