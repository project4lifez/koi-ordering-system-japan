using System;
using System.Collections.Generic;

namespace KoiOrderingSystem.Models;

public partial class Booking
{
    public int BookingId { get; set; }

    public int? CustomerId { get; set; }

    public int? RoleId { get; set; }

    public int? TripId { get; set; }

    public int? PoId { get; set; }

    public int? BookingPaymentId { get; set; }

    public int? FormOrderId { get; set; }

    public int? FeedbackId { get; set; }

    public string? Status { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public DateOnly? KoiDeliveryDate { get; set; }

    public TimeOnly? KoiDeliveryTime { get; set; }

    public decimal? TotalAmount { get; set; }

    public virtual BookingPayment? BookingPayment { get; set; }

    public virtual ICollection<BookingPayment> BookingPayments { get; set; } = new List<BookingPayment>();

    public virtual Customer? Customer { get; set; }

    public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>();

    public virtual Feedback? Feedback { get; set; }

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual ICollection<FormBooking> FormBookings { get; set; } = new List<FormBooking>();

    public virtual FormBooking? FormOrder { get; set; }

    public virtual Po? Po { get; set; }

    public virtual ICollection<Po> Pos { get; set; } = new List<Po>();

    public virtual Role? Role { get; set; }

    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();

    public virtual Trip? Trip { get; set; }

    public virtual ICollection<Trip> Trips { get; set; } = new List<Trip>();
}
