using FluentValidation;

namespace ElroukenAljamil.Search.Application.Queries.SearchListings
{
    public class SearchListingsQueryValidator : AbstractValidator<SearchListingsQuery>
    {
        private static readonly string[] AllowedSortValues =
            { "relevance", "price_asc", "price_desc", "date", "distance" };

        public SearchListingsQueryValidator()
        {
            RuleFor(x => x.Page)
                .GreaterThanOrEqualTo(1).WithMessage("La page doit être >= 1.");

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100).WithMessage("La taille de page doit être entre 1 et 100.");

            RuleFor(x => x.MinPrice)
                .GreaterThanOrEqualTo(0).When(x => x.MinPrice.HasValue)
                .WithMessage("Le prix minimum ne peut pas être négatif.");

            RuleFor(x => x.MaxPrice)
                .GreaterThan(x => x.MinPrice ?? 0).When(x => x.MaxPrice.HasValue && x.MinPrice.HasValue)
                .WithMessage("Le prix maximum doit être supérieur au prix minimum.");

            RuleFor(x => x.RadiusKm)
                .InclusiveBetween(1, 500).When(x => x.RadiusKm.HasValue)
                .WithMessage("Le rayon doit être entre 1 et 500 km.");

            RuleFor(x => x.Latitude)
                .InclusiveBetween(-90, 90).When(x => x.Latitude.HasValue)
                .WithMessage("La latitude doit être entre -90 et 90.");

            RuleFor(x => x.Longitude)
                .InclusiveBetween(-180, 180).When(x => x.Longitude.HasValue)
                .WithMessage("La longitude doit être entre -180 et 180.");

            RuleFor(x => x.SortBy)
                .Must(sort => AllowedSortValues.Contains(sort.ToLowerInvariant()))
                .WithMessage($"Tri autorisé : {string.Join(", ", AllowedSortValues)}");

            // Si tri par distance, les coordonnées sont obligatoires
            RuleFor(x => x.Latitude)
                .NotNull().When(x => x.SortBy == "distance")
                .WithMessage("La latitude est requise pour le tri par distance.");

            RuleFor(x => x.Longitude)
                .NotNull().When(x => x.SortBy == "distance")
                .WithMessage("La longitude est requise pour le tri par distance.");
        }
    }


}
