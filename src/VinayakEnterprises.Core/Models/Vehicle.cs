using System;

namespace VinayakEnterprises.Core.Models;

public class Vehicle : BaseEntity
{
    public string VehicleNo { get; set; } = string.Empty;
    public int? TareWtKg { get; set; }
    public int? RTOWtKg { get; set; }
    public DateTime? TareDate { get; set; }
    public int? MaxTareAllow { get; set; }
    public int? MinTareAllow { get; set; }
    public bool IsBlacklist { get; set; }
}
