using System.ComponentModel.DataAnnotations;

namespace Galaxon.Astronomy.Data.Models;

public class AstroObjectGroup
{
    // Primary key.
    public int Id { get; set; }

    // Group name.
    [MaxLength(20)]
    public string Name { get; set; } = "";

    // Objects in the group (navigation property).
    public List<AstroObject> Objects { get; set; } = [];

    // Parent group.
    public int? ParentId { get; set; }

    public AstroObjectGroup? Parent { get; set; }
}
