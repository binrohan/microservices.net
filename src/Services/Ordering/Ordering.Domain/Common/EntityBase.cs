using System;

namespace Ordering.Domain.Common;

public abstract class EntityBase
{
    public int Id { get; set; }
    public string CreatedBy { get; set; } = null!;
    public DateTime CreatedDate { get; set; }
    public string LastModifiedBy { get; set; } = null!;
    public DateTime? LastModifiedDate { get; set; }
}
