using System;
using System.Collections.Generic;

namespace KoiOrderingSystem.Models;

public partial class Po
{
    public int PoId { get; set; }

    public int? BookingId { get; set; }

    public int? FarmId { get; set; }

    public int? PoDetailId { get; set; }

    public int? PoPaymentId { get; set; }

    public decimal? TotalAmount { get; set; }

    public virtual Booking? Booking { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<KoiFarm> KoiFarms { get; set; } = new List<KoiFarm>();

    public virtual Podetail? PoDetail { get; set; }

    public virtual Popayment? PoPayment { get; set; }

    public virtual ICollection<Podetail> Podetails { get; set; } = new List<Podetail>();

    public virtual ICollection<Popayment> Popayments { get; set; } = new List<Popayment>();
}
