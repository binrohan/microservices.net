using System;

namespace Ordering.Domain.Common;

public abstract class EntityBase
{
    public int Id { get; set; }
    public required string CreateBy { get; set; }
    public DateTime CreatedDate { get; set; }
    public required string LastModifiedBy { get; set; }
    public DateTime? LastModifiedDate { get; set; }
}
