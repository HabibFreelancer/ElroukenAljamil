using ElroukenAljamil.Identity.Domain.Interfaces;
using MediatR;

namespace ElroukenAljamil.Identity.Application.Commands.CheckEmail
{
    public record CheckEmailCommand(string Email) : IRequest<bool>;

    public class CheckEmailCommandHandler : IRequestHandler<CheckEmailCommand, bool>
    {
        private readonly IUserRepository _userRepository;
        public CheckEmailCommandHandler(IUserRepository userRepository) => _userRepository = userRepository;

        public Task<bool> Handle(CheckEmailCommand request, CancellationToken ct) =>
            _userRepository.EmailExistsAsync(request.Email.ToLowerInvariant().Trim(), ct);
    }
}
