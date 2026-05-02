using System;

namespace VinayakEnterprises.Core.Models;

public class AuditLog : BaseEntity
{
    public DateTime Timestamp { get; set; }
    public int? UserId { get; set; }
    public string? UserName { get; set; }
    public string Action { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public string? EntityId { get; set; }
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public string? IPAddress { get; set; }
    public string? PCName { get; set; }
}
