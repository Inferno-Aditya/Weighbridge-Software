namespace VinayakEnterprises.Core.Models;

public class GoodsDispatch : BaseEntity
{
    public string PartyName { get; set; } = string.Empty;
    public string VehicleNo { get; set; } = string.Empty;
    public string TicketNo { get; set; } = string.Empty;
    public decimal? Gross { get; set; }
    public decimal? Tare { get; set; }
    public decimal? Net { get; set; }
    public string? Item { get; set; }
}
