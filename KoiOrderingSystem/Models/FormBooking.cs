using System;
using System.Collections.Generic;

namespace KoiOrderingSystem.Models;

public partial class FormBooking
{
    public int FormBookingId { get; set; }

    public int? BookingId { get; set; }

    public int? CustomerId { get; set; }

    public string? Fullname { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? Favoritefarm { get; set; }

    public decimal? EstimatedCost { get; set; }

    public string? FavoriteKoi { get; set; }

    public string? HotelAccommodation { get; set; }

    public DateOnly? EstimatedDepartureDate { get; set; }

    public DateOnly? ReturnDate { get; set; }

    public string? Gender { get; set; }

    public string? Note { get; set; }

    public virtual Booking? Booking { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual Customer? Customer { get; set; }
}
