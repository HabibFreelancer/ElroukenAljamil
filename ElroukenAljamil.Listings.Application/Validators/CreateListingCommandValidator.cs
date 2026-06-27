using FluentValidation;
using ElroukenAljamil.Listings.Application.Commands;


namespace ElroukenAljamil.Listings.Application.Validators
{
    /// <summary>
    /// Validation des données d'entrée avant traitement métier.
    /// Sépare la validation technique de la validation métier (domaine).
    /// </summary>
    public class CreateListingCommandValidator : AbstractValidator<CreateListingCommand>
    {
        public CreateListingCommandValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Le titre est obligatoire.")
                .MaximumLength(150).WithMessage("Le titre ne doit pas dépasser 150 caractères.");


            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("La description est obligatoire.")
                .MaximumLength(5000).WithMessage("La description ne doit pas dépasser 5000 caractères.");


            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Le prix doit être supérieur à 0.");


            RuleFor(x => x.Currency)
                .NotEmpty()
                .Length(3).WithMessage("Le code devise doit contenir 3 caractères.");


            RuleFor(x => x.SellerId)
                .NotEmpty().WithMessage("L'identifiant du vendeur est obligatoire.");


            RuleFor(x => x.CategoryId)
                .NotEmpty().WithMessage("La catégorie est obligatoire.");


            RuleFor(x => x.City)
                .NotEmpty().WithMessage("La ville est obligatoire.");


            RuleFor(x => x.PostalCode)
                .NotEmpty().WithMessage("Le code postal est obligatoire.");
        }
    }

}
