using System;
using System.Collections.Generic;

namespace KoiOrderingSystem.Models;

public partial class Popayment
{
    public int PoPaymentId { get; set; }

    public int? PoId { get; set; }

    public decimal? Amount { get; set; }

    public DateOnly? PaymentDate { get; set; }

    public int? PaymentMethodId { get; set; }

    public string? Status { get; set; }

    public virtual PaymentMethod? PaymentMethod { get; set; }

    public virtual ICollection<PaymentMethod> PaymentMethods { get; set; } = new List<PaymentMethod>();

    public virtual Po? Po { get; set; }

    public virtual ICollection<Po> Pos { get; set; } = new List<Po>();
}
