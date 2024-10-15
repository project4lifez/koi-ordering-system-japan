using System;
using System.Collections.Generic;

namespace KoiOrderingSystem.Models;

public partial class Popayment
{
    public int PoPaymentId { get; set; }

    public int? PoId { get; set; }

    public DateOnly? PaymentDate { get; set; }

    public virtual Po? Po { get; set; }
}
