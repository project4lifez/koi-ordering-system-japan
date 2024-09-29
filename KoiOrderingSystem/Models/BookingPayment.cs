using System;
using System.Collections.Generic;

namespace KoiOrderingSystem.Models;

public partial class BookingPayment
{
    public int BookingPaymentId { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<PaymentMethod> PaymentMethods { get; set; } = new List<PaymentMethod>();
}
