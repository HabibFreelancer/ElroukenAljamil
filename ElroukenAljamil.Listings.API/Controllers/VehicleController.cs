using System.Text.RegularExpressions;
using ElroukenAljamil.Listings.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ElroukenAljamil.Listings.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VehicleController : ControllerBase
{
    private readonly IVehicleService _vehicleService;

    public VehicleController(IVehicleService vehicleService) => _vehicleService = vehicleService;

    [HttpGet("lookup/{immatriculation}")]
    public async Task<ActionResult> Lookup(string immatriculation, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(immatriculation))
            return BadRequest(new { message = "Immatriculation requise." });

        var normalized = immatriculation.Replace(" ", "").ToUpper();

        if (!Regex.IsMatch(normalized, @"^\d{1,3}(TU|TUNES)\d{1,4}$", RegexOptions.IgnoreCase))
            return BadRequest(new { message = "Format invalide." });

        var result = await _vehicleService.LookupAsync(normalized, ct);
        return Ok(result);
    }
}
