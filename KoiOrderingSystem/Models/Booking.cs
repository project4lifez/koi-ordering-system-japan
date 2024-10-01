using System;
using System.Collections.Generic;

namespace KoiOrderingSystem.Models;

public partial class Booking
{
    public int BookingId { get; set; }

    public int? TripId { get; set; }

    public int? PoId { get; set; }

    public int? BookingPaymentId { get; set; }

    public int? FormBookingId { get; set; }

    public int? FeedbackId { get; set; }

    public decimal? QuotedAmount { get; set; }

    public DateOnly? QuoteSentDate { get; set; }

    public DateOnly? QuoteApprovedDate { get; set; }

    public bool? ManagerApproval { get; set; }

    public string? Status { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public DateOnly? KoiDeliveryDate { get; set; }

    public TimeOnly? KoiDeliveryTime { get; set; }

    public decimal? TotalAmount { get; set; }

    public DateTime? BookingDate { get; set; }

    public virtual BookingPayment? BookingPayment { get; set; }

    public virtual Feedback? Feedback { get; set; }

    public virtual FormBooking? FormBooking { get; set; }

    public virtual Po? Po { get; set; }

    public virtual Trip? Trip { get; set; }
}
