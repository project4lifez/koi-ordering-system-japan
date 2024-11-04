using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace KoiOrderingSystem.Models;

public partial class Variety
{
    public int VarietyId { get; set; }

    public int? PackageId { get; set; }

    public string? VarietyName { get; set; }

    public string? Description { get; set; }

    public string? ImageUrl { get; set; }

    // Map 'MainTopics' property to 'Maintopic' column in the database
    [Column("maintopic")] // Maps MainTopics to maintopic in the database
    public string? MainTopics { get; set; }

    // Map 'SubTopics' property to 'Subtopic' column in the database
    [Column("subtopic")] // Maps SubTopics to subtopic in the database
    public string? SubTopics { get; set; }

    public virtual ICollection<KoiFish> KoiFishes { get; set; } = new List<KoiFish>();

    public virtual KoiPackage? Package { get; set; }

    public virtual ICollection<SpecialVariety> SpecialVarieties { get; set; } = new List<SpecialVariety>();
}
