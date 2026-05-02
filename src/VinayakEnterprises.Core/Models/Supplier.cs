namespace VinayakEnterprises.Core.Models;

public class Supplier : BaseEntity
{
    public string CodeNo { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Phone { get; set; }
    public string? Mobile { get; set; }
    public string? VATNo { get; set; }
    public string? Email { get; set; }
    public string? W_Charges { get; set; }
    public string? Website { get; set; }
    public bool IsBlacklist { get; set; }
}
