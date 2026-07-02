using FluentValidation;

namespace ElroukenAljamil.Messaging.Application.Commands.StartConversation
{
    public class StartConversationCommandValidator : AbstractValidator<StartConversationCommand>
    {
        public StartConversationCommandValidator()
        {
            RuleFor(x => x.SellerId)
                .NotEmpty().WithMessage("Le vendeur est obligatoire.");

            RuleFor(x => x.ListingId)
                .NotEmpty().WithMessage("L'annonce est obligatoire.");

            RuleFor(x => x.ListingTitle)
                .NotEmpty().WithMessage("Le titre de l'annonce est obligatoire.");

            RuleFor(x => x.Message)
                .NotEmpty().WithMessage("Le message est obligatoire.")
                .MaximumLength(2000).WithMessage("Le message ne peut pas dépasser 2000 caractères.");
        }
    }
}
