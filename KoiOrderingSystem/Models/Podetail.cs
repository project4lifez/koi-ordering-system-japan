using System;
using System.Collections.Generic;

namespace KoiOrderingSystem.Models;

public partial class Podetail
{
    public int PoDetailId { get; set; }

    public int? PoId { get; set; }

    public int? KoiId { get; set; }

    public int? PackageId { get; set; }

    public int? FarmId { get; set; }

    public decimal? Deposit { get; set; }

    public decimal? TotalKoiPrice { get; set; }

    public int? Quantity { get; set; }

    public string? ImageUrl { get; set; }

    public virtual KoiFarm? Farm { get; set; }

    public virtual KoiFish? Koi { get; set; }

    public virtual ICollection<KoiFish> KoiFishes { get; set; } = new List<KoiFish>();

    public virtual ICollection<Koipackage> Koipackages { get; set; } = new List<Koipackage>();

    public virtual Koipackage? Package { get; set; }

    public virtual Po? Po { get; set; }

    public virtual ICollection<Po> Pos { get; set; } = new List<Po>();
}
