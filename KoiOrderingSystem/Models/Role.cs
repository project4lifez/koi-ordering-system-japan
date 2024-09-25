using System;
using System.Collections.Generic;

namespace KoiOrderingSystem.Models;

public partial class Role
{
    public int RoleId { get; set; }

    public string? Name { get; set; }

    public int? AccountId { get; set; }

    public int? BookingId { get; set; }

    public virtual Account? Account { get; set; }

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();

    public virtual Booking? Booking { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
