﻿using System;
using System.Collections.Generic;

namespace KoiOrderingSystem.Models;

public partial class Trip
{
    public int TripId { get; set; }

    public int? TripDetailId { get; set; }

    public string? TripName { get; set; }

    public string? Status { get; set; }

    public decimal? Price { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual TripDetail? TripDetail { get; set; }
}
