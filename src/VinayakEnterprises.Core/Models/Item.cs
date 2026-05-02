namespace VinayakEnterprises.Core.Models;

public class Item : BaseEntity
{
    public string CodeNo { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal? Price { get; set; }
    public decimal? Weight { get; set; }
    public string? Unit { get; set; }
}
