using ElroukenAljamil.Listings.Application.DTOs;
using ElroukenAljamil.Listings.Domain.Entities;
using ElroukenAljamil.Listings.Domain.Interfaces;
using MediatR;

namespace ElroukenAljamil.Listings.Application.Commands.CreateAdType
{
    public record CreateAdTypeCommand(CreateAdTypeRequest Request) : IRequest<AdTypeDto>;

    public class CreateAdTypeCommandHandler : IRequestHandler<CreateAdTypeCommand, AdTypeDto>
    {
        private readonly IAdTypeRepository _repository;
        public CreateAdTypeCommandHandler(IAdTypeRepository repository) => _repository = repository;

        public async Task<AdTypeDto> Handle(CreateAdTypeCommand request, CancellationToken ct)
        {
            var r = request.Request;
            var adType = AnnonceAdType.Create(r.CategoryId, r.Label, r.Description, r.DisplayOrder, r.IsDefault);
            await _repository.AddAsync(adType, ct);

            return new AdTypeDto
            {
                Id = adType.Id,
                CategoryId = adType.CategoryId,
                Label = adType.Label,
                Description = adType.Description,
                IsDefault = adType.IsDefault,
                DisplayOrder = adType.DisplayOrder,
                IsActive = adType.IsActive
            };
        }
    }
}
