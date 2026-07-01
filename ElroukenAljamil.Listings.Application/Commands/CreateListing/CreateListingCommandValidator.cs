using FluentValidation;

namespace ElroukenAljamil.Listings.Application.Commands.CreateListing
{
    public class CreateListingCommandValidator : AbstractValidator<CreateListingCommand>
    {
        public CreateListingCommandValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Le titre est obligatoire.")
                .MaximumLength(200).WithMessage("Le titre ne peut pas dépasser 200 caractères.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("La description est obligatoire.")
                .MaximumLength(5000).WithMessage("La description ne peut pas dépasser 5000 caractères.");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Le prix doit être supérieur à 0.");

            RuleFor(x => x.Currency)
                .NotEmpty().WithMessage("La devise est obligatoire.")
                .Length(3).WithMessage("La devise doit faire 3 caractères (ISO 4217).");

            RuleFor(x => x.Category)
                .NotEmpty().WithMessage("La catégorie est obligatoire.");

            RuleFor(x => x.City)
                .NotEmpty().WithMessage("La ville est obligatoire.");

            RuleFor(x => x.ImageUrls)
                .Must(urls => urls.Count <= 15).WithMessage("Maximum 15 images par annonce.");

            RuleFor(x => x.Latitude)
                .InclusiveBetween(-90, 90).When(x => x.Latitude.HasValue)
                .WithMessage("La latitude doit être entre -90 et 90.");

            RuleFor(x => x.Longitude)
                .InclusiveBetween(-180, 180).When(x => x.Longitude.HasValue)
                .WithMessage("La longitude doit être entre -180 et 180.");
        }
    }

}
