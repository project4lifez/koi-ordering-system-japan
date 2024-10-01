using System;
using System.Collections.Generic;

namespace KoiOrderingSystem.Models;

public partial class TripDetail
{
    public int TripDetailId { get; set; }

    public string? MainTopic { get; set; }

    public string? SubTopic { get; set; }

    public decimal? NotePrice { get; set; }

    public DateOnly? Day { get; set; }

    public virtual ICollection<KoiFarm> KoiFarms { get; set; } = new List<KoiFarm>();

    public virtual ICollection<Trip> Trips { get; set; } = new List<Trip>();
}
