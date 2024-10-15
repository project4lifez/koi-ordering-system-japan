using System;
using System.Collections.Generic;

namespace KoiOrderingSystem.Models;

public partial class Booking
{
    public int BookingId { get; set; }

    public int? TripId { get; set; }

    public int? PoId { get; set; }

    public int? BookingPaymentId { get; set; }

    public int? FeedbackId { get; set; }

    public int? CustomerId { get; set; }

    public DateOnly? QuoteSentDate { get; set; }

    public DateOnly? QuoteApprovedDate { get; set; }

    public string? Status { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public decimal? QuotedAmount { get; set; }

    public DateOnly? BookingDate { get; set; }

    public string? Fullname { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? Favoritefarm { get; set; }

    public decimal? EstimatedCost { get; set; }

    public string? FavoriteKoi { get; set; }

    public string? Gender { get; set; }

    public string? Note { get; set; }

    public string? HotelAccommodation { get; set; }

    public bool? IsActive { get; set; }

    public virtual BookingPayment? BookingPayment { get; set; }

    public virtual ICollection<BookingPayment> BookingPayments { get; set; } = new List<BookingPayment>();

    public virtual Customer? Customer { get; set; }

    public virtual Feedback? Feedback { get; set; }

    public virtual Po? Po { get; set; }

    public virtual Trip? Trip { get; set; }
}
