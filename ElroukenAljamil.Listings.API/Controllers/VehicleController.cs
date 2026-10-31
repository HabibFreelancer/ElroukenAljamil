using System.Text.RegularExpressions;
using ElroukenAljamil.Listings.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ElroukenAljamil.Listings.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VehicleController : ControllerBase
{
    private readonly IVehicleService _vehicleService;
    private readonly ILogger<VehicleController> _logger;

    public VehicleController(IVehicleService vehicleService, ILogger<VehicleController> logger)
    {
        _vehicleService = vehicleService;
        _logger = logger;
    }

    [HttpGet("lookup/{immatriculation}")]
    public async Task<ActionResult> Lookup(string immatriculation, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(immatriculation))
        {
            _logger.LogWarning("Lookup véhicule — immatriculation manquante");
            return BadRequest(new { message = "Immatriculation requise." });
        }

        var normalized = immatriculation.Replace(" ", "").ToUpper();

        if (!Regex.IsMatch(normalized, @"^\d{1,3}(TU|TUNES)\d{1,4}$", RegexOptions.IgnoreCase))
        {
            _logger.LogWarning("Lookup véhicule — format invalide : {Immatriculation}", normalized);
            return BadRequest(new { message = "Format invalide." });
        }

        _logger.LogInformation("Lookup véhicule pour immatriculation {Immatriculation}", normalized);
        var result = await _vehicleService.LookupAsync(normalized, ct);
        return Ok(result);
    }
}
