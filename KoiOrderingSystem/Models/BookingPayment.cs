using System;
using System.Collections.Generic;

namespace KoiOrderingSystem.Models;

public partial class BookingPayment
{
    public int BookingPaymentId { get; set; }

    public string? Status { get; set; }

    public int? PaymentMethodId { get; set; }

    public DateTime? PaymentDate { get; set; }

    public int? BookingId { get; set; }

    public virtual Booking? Booking { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual PaymentMethod? PaymentMethod { get; set; }
}
