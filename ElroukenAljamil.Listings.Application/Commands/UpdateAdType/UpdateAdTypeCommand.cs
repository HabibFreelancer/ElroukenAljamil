using ElroukenAljamil.Listings.Application.DTOs;
using ElroukenAljamil.Listings.Domain.Interfaces;
using MediatR;

namespace ElroukenAljamil.Listings.Application.Commands.UpdateAdType
{
    public record UpdateAdTypeCommand(int Id, UpdateAdTypeRequest Request) : IRequest<bool>;

    public class UpdateAdTypeCommandHandler : IRequestHandler<UpdateAdTypeCommand, bool>
    {
        private readonly IAdTypeRepository _repository;
        public UpdateAdTypeCommandHandler(IAdTypeRepository repository) => _repository = repository;

        public async Task<bool> Handle(UpdateAdTypeCommand request, CancellationToken ct)
        {
            var adType = await _repository.GetByIdAsync(request.Id, ct);
            if (adType is null) return false;

            var r = request.Request;
            adType.Update(r.CategoryId, r.Label, r.Description, r.DisplayOrder, r.IsDefault, r.IsActive);
            await _repository.UpdateAsync(adType, ct);
            return true;
        }
    }
}
