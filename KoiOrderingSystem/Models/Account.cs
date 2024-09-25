using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KoiOrderingSystem.Models;

public partial class Account
{
    public int AccountId { get; set; }
    
    public string Username { get; set; } = null!;
    
    public string Password { get; set; } = null!;

    public string? Lastname { get; set; }
    
    public string? Firstname { get; set; }

    public string? Gender { get; set; }

    public string? Phone { get; set; }
    
    
    public string? Email { get; set; }

    public string? ImageUrl { get; set; }

    public int? RoleId { get; set; }

    public bool? Status { get; set; }

    public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>();

    public virtual Role? Role { get; set; }

    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
}
