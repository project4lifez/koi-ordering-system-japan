using System;
using System.Collections.Generic;

namespace KoiOrderingSystem.Models;

public partial class Variety
{
    public int VarietyId { get; set; }

    public int? SpecialVarietyId { get; set; }

    public int? KoiId { get; set; }

    public int? PackageId { get; set; }

    public string? VarietyName { get; set; }

    public string? Description { get; set; }

    public virtual KoiFish? Koi { get; set; }

    public virtual ICollection<KoiFish> KoiFishes { get; set; } = new List<KoiFish>();

    public virtual ICollection<Koipackage> Koipackages { get; set; } = new List<Koipackage>();

    public virtual Koipackage? Package { get; set; }

    public virtual ICollection<SpecialVariety> SpecialVarieties { get; set; } = new List<SpecialVariety>();

    public virtual SpecialVariety? SpecialVariety { get; set; }
}
