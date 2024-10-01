using System;
using System.Collections.Generic;

namespace KoiOrderingSystem.Models;

public partial class Customer
{
    public int CustomerId { get; set; }

    public int? AccountId { get; set; }

    public virtual Account? Account { get; set; }

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual ICollection<FormBooking> FormBookings { get; set; } = new List<FormBooking>();
}
