using System;
using System.Collections.Generic;

namespace KoiOrderingSystem.Models;

public partial class PaymentMethod
{
    public int PaymentMethodId { get; set; }

    public string? MethodName { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<BookingPayment> BookingPayments { get; set; } = new List<BookingPayment>();

    public virtual ICollection<Popayment> Popayments { get; set; } = new List<Popayment>();
}
