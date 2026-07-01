using ElroukenAljamil.BuildingBlocks.Security;
using ElroukenAljamil.BuildingBlocks.Security.Services;
using ElroukenAljamil.Identity.Application.DTOs;
using ElroukenAljamil.Identity.Domain.Interfaces;
using ElroukenAljamil.Identity.Domain.ValueObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElroukenAljamil.Identity.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly ICurrentUserService _currentUser;

        public ProfileController(IUserRepository userRepository, ICurrentUserService currentUser)
        {
            _userRepository = userRepository;
            _currentUser = currentUser;
        }

        /// <summary>
        /// Récupère le profil de l'utilisateur connecté.
        /// </summary>
        [HttpGet("me")]
        [ProducesResponseType(typeof(UserProfileDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMyProfile(CancellationToken ct)
        {
            var user = await _userRepository.GetByIdAsync(_currentUser.UserId, ct);
            if (user is null)
                return NotFound(new { error = "Utilisateur introuvable." });

            var profile = new UserProfileDto
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                Role = user.Role.ToString(),
                Status = user.Status.ToString(),
                AvatarUrl = user.AvatarUrl,
                CreatedAt = user.CreatedAt
            };

            return Ok(profile);
        }

        /// <summary>
        /// Met à jour le profil de l'utilisateur connecté.
        /// </summary>
        [HttpPut("me")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdateProfile(
            [FromBody] UpdateProfileRequest request, CancellationToken ct)
        {
            var user = await _userRepository.GetByIdAsync(_currentUser.UserId, ct);
            if (user is null)
                return NotFound(new { error = "Utilisateur introuvable." });

            Address? address = null;
            if (request.Address is not null)
            {
                address = new Address(
                    request.Address.Street,
                    request.Address.City,
                    request.Address.ZipCode,
                    request.Address.Country,
                    request.Address.State);
            }

            user.UpdateProfile(request.FirstName, request.LastName, request.PhoneNumber, address);
            await _userRepository.UpdateAsync(user, ct);

            return NoContent();
        }
    }

    public record UpdateProfileRequest
    {
        public string FirstName { get; init; } = string.Empty;
        public string LastName { get; init; } = string.Empty;
        public string? PhoneNumber { get; init; }
        public AddressRequest? Address { get; init; }
    }

    public record AddressRequest
    {
        public string Street { get; init; } = string.Empty;
        public string City { get; init; } = string.Empty;
        public string ZipCode { get; init; } = string.Empty;
        public string Country { get; init; } = string.Empty;
        public string? State { get; init; }
    }

}
