using System;
using System.Collections.Generic;

namespace KoiOrderingSystem.Models;

public partial class KoiPackage
{
    public int PackageId { get; set; }

    public int? FarmId { get; set; }

    public int? PoDetailId { get; set; }

    public string? PackageName { get; set; }

    public string? Description { get; set; }

    public decimal? PackagePrice { get; set; }

    public string? ImageUrl { get; set; }

    public virtual KoiFarm? Farm { get; set; }

    public virtual Podetail? PoDetail { get; set; }

    public virtual ICollection<Variety> Varieties { get; set; } = new List<Variety>();
}
