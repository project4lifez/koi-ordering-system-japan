using System;
using System.Collections.Generic;

namespace KoiOrderingSystem.Models;

public partial class Po
{
    public int PoId { get; set; }

    public int? FarmId { get; set; }

    public decimal? TotalAmount { get; set; }

    public DateOnly? KoiDeliveryDate { get; set; }

    public TimeOnly? KoiDeliveryTime { get; set; }

    public string? Status { get; set; }

    public int? PoDetailId { get; set; }

    public string? DeliveryLocation { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual KoiFarm? Farm { get; set; }

    public virtual Podetail? PoDetail { get; set; }

    public virtual ICollection<Podetail> Podetails { get; set; } = new List<Podetail>();

    public virtual ICollection<Popayment> Popayments { get; set; } = new List<Popayment>();
}
