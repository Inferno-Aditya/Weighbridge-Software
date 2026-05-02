namespace VinayakEnterprises.Core.Models;

public class SystemSettings
{
    public int Id { get; set; }
    public string Theme { get; set; } = "dark";
    public string Language { get; set; } = "EN";
    public int CameraIndex { get; set; } = 0;
    public string? DefaultPrinter { get; set; }
    public int SessionTimeout { get; set; } = 30;
    public int StableWeightThreshold { get; set; } = 2;
}
