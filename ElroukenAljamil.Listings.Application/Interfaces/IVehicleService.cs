namespace ElroukenAljamil.Listings.Application.Interfaces;

public interface IVehicleService
{
    Task<VehicleLookupResult> LookupAsync(string immatriculation, CancellationToken ct = default);
}

public class VehicleLookupResult
{
    public string? Brand { get; set; }
    public string? Model { get; set; }
    public string? Year { get; set; }
    public string? Fuel { get; set; }
    public string? Gearbox { get; set; }
    public string? FiscalPower { get; set; }
    public string? DinPower { get; set; }
    public string? FirstCirculation { get; set; }
    public string? Color { get; set; }
    public string? VehicleType { get; set; }
}
