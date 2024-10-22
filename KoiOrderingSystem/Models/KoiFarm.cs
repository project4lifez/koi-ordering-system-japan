using System;
using System.Collections.Generic;

namespace KoiOrderingSystem.Models;

public partial class KoiFarm
{
    public int FarmId { get; set; }

    public int? TripDetailId { get; set; }

    public int? KoiId { get; set; }

    public string? FarmName { get; set; }

    public string? Location { get; set; }

    public string? ContactInfo { get; set; }

    public string? ImageUrl { get; set; }

    public int? SpecialVarietyId { get; set; }

    public virtual KoiFish? Koi { get; set; }

    public virtual ICollection<KoiPackage> KoiPackages { get; set; } = new List<KoiPackage>();

    public virtual ICollection<Podetail> Podetails { get; set; } = new List<Podetail>();

    public virtual ICollection<Po> Pos { get; set; } = new List<Po>();

    public virtual ICollection<SpecialVariety> SpecialVarieties { get; set; } = new List<SpecialVariety>();

}
