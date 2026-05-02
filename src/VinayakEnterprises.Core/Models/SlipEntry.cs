using System;

namespace VinayakEnterprises.Core.Models;

public class SlipEntry : BaseEntity
{
    public string TicketNo { get; set; } = string.Empty;
    public int? CustomerId { get; set; }
    public int? SupplierId { get; set; }
    public string VehicleNo { get; set; } = string.Empty;
    public int? ItemId { get; set; }
    public int? OperatorId { get; set; }
    public string? WBLocation { get; set; }
    public int? GrossWt { get; set; }
    public int? TareWt { get; set; }
    public int? NetWt { get; set; }
    public DateTime? GrossTime { get; set; }
    public DateTime? TareTime { get; set; }
    public string? CameraImagePath { get; set; }
    public int? Field01Id { get; set; }
    public int? Field02Id { get; set; }
    public int? Field03Id { get; set; }
    public string TicketStatus { get; set; } = "New"; // "New", "Completed", "Deleted"
    public int WeighmentNo { get; set; } = 1; // 1 or 2
    public bool ManualData { get; set; } = false;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
