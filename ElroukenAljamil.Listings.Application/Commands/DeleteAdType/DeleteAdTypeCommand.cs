using ElroukenAljamil.Listings.Domain.Interfaces;
using MediatR;

namespace ElroukenAljamil.Listings.Application.Commands.DeleteAdType
{
    public record DeleteAdTypeCommand(int Id) : IRequest<bool>;

    public class DeleteAdTypeCommandHandler : IRequestHandler<DeleteAdTypeCommand, bool>
    {
        private readonly IAdTypeRepository _repository;
        public DeleteAdTypeCommandHandler(IAdTypeRepository repository) => _repository = repository;

        public async Task<bool> Handle(DeleteAdTypeCommand request, CancellationToken ct)
        {
            var adType = await _repository.GetByIdAsync(request.Id, ct);
            if (adType is null) return false;

            await _repository.DeleteAsync(adType, ct);
            return true;
        }
    }
}
