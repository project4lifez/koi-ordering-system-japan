using System;
using System.Collections.Generic;

namespace KoiOrderingSystem.Models;

public partial class Blog
{
    public int BlogId { get; set; }

    public string Heading { get; set; } = null!;

    public string Image { get; set; } = null!;

    public string Link { get; set; } = null!;

    public bool Status { get; set; }

    public int Position { get; set; }

    public DateTime? CreateAt { get; set; }
}
