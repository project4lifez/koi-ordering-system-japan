using System;
using System.Collections.Generic;

namespace KoiOrderingSystem.Models;

public partial class Feedback
{
    public int FeedbackId { get; set; }

    public int? BookingId { get; set; }

    public int? Rating { get; set; }

    public string? Comments { get; set; }

    public int? CustomerId { get; set; }

    public virtual Booking? Booking { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual Customer? Customer { get; set; }
}
