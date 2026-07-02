using FluentValidation;

namespace ElroukenAljamil.Messaging.Application.Commands.SendMessage
{
    public class SendMessageCommandValidator : AbstractValidator<SendMessageCommand>
    {
        public SendMessageCommandValidator()
        {
            RuleFor(x => x.ConversationId)
                .NotEmpty().WithMessage("La conversation est obligatoire.");

            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Le message ne peut pas être vide.")
                .MaximumLength(2000).WithMessage("Le message ne peut pas dépasser 2000 caractères.");
        }
    }
}
