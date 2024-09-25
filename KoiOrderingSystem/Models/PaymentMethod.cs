using System;
using System.Collections.Generic;

namespace KoiOrderingSystem.Models;

public partial class PaymentMethod
{
    public int PaymentMethodId { get; set; }

    public int? BookingPaymentId { get; set; }

    public int? PoPaymentId { get; set; }

    public string? MethodName { get; set; }

    public string? Description { get; set; }

    public virtual BookingPayment? BookingPayment { get; set; }

    public virtual ICollection<BookingPayment> BookingPayments { get; set; } = new List<BookingPayment>();

    public virtual Popayment? PoPayment { get; set; }

    public virtual ICollection<Popayment> Popayments { get; set; } = new List<Popayment>();
}
