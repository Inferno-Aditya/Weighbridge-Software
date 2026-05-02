namespace VinayakEnterprises.Core.Models;

public class HelpCode : BaseEntity
{
    public string EntityType { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}
