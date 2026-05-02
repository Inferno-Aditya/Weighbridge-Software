using System;

namespace VinayakEnterprises.Core.Models;

public class LicenseInfo
{
    public int Id { get; set; }
    public string MachineId { get; set; } = string.Empty;
    public string LicenseType { get; set; } = string.Empty;
    public DateTime IssueDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    public string? ActivatedBy { get; set; }
    public DateTime? ActivationDate { get; set; }
}
