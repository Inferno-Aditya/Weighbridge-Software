namespace VinayakEnterprises.Core.Models;

public class CompanyMaster
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? LogoPath { get; set; }
    public string? GSTNo { get; set; }
    public string? Phone { get; set; }
}
