using System;
using System.Collections.Generic;

namespace KoiOrderingSystem.Models;

public partial class Podetail
{
    public int PoDetailId { get; set; }

    public int? PoId { get; set; }

    public int? KoiId { get; set; }

    public int? FarmId { get; set; }

    public decimal? Deposit { get; set; }

    public decimal? TotalKoiPrice { get; set; }

    public decimal? RemainingPrice { get; set; }

    public int? Quantity { get; set; }

    public string? ImageUrl { get; set; }

    public DateOnly? Day { get; set; }

    public virtual KoiFarm? Farm { get; set; }

    public virtual KoiFish? Koi { get; set; }

    public virtual ICollection<KoiPackage> KoiPackages { get; set; } = new List<KoiPackage>();

    public virtual Po? Po { get; set; }

    public virtual ICollection<Po> Pos { get; set; } = new List<Po>();
}
