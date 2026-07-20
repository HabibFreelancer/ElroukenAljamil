using ElroukenAljamil.Identity.Application.DTOs;
using ElroukenAljamil.Identity.Domain.Interfaces;
using MediatR;

namespace ElroukenAljamil.Identity.Application.Queries.GetCurrentUser
{
    public record GetCurrentUserQuery(Guid UserId) : IRequest<UserProfileDto?>;

    public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, UserProfileDto?>
    {
        private readonly IUserRepository _userRepository;
        public GetCurrentUserQueryHandler(IUserRepository userRepository) => _userRepository = userRepository;

        public async Task<UserProfileDto?> Handle(GetCurrentUserQuery request, CancellationToken ct)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId, ct);
            if (user is null) return null;

            return new UserProfileDto
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
        }
    }
}
