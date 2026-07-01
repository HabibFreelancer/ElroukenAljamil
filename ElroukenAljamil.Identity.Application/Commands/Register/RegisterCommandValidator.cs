using FluentValidation;

namespace ElroukenAljamil.Identity.Application.Commands.Register
{
    public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
    {
        public RegisterCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("L'email est obligatoire.")
                .EmailAddress().WithMessage("Format d'email invalide.");

            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("Le nom d'utilisateur est obligatoire.")
                .MinimumLength(3).WithMessage("Minimum 3 caractères.")
                .MaximumLength(50).WithMessage("Maximum 50 caractères.")
                .Matches(@"^[a-zA-Z0-9_-]+$").WithMessage("Caractères autorisés : lettres, chiffres, _ et -");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Le mot de passe est obligatoire.")
                .MinimumLength(8).WithMessage("Minimum 8 caractères.")
                .Matches(@"[A-Z]").WithMessage("Au moins une majuscule requise.")
                .Matches(@"[a-z]").WithMessage("Au moins une minuscule requise.")
                .Matches(@"[0-9]").WithMessage("Au moins un chiffre requis.")
                .Matches(@"[\W_]").WithMessage("Au moins un caractère spécial requis.");

            RuleFor(x => x.ConfirmPassword)
                .Equal(x => x.Password).WithMessage("Les mots de passe ne correspondent pas.");

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("Le prénom est obligatoire.")
                .MaximumLength(100);

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Le nom est obligatoire.")
                .MaximumLength(100);
        }
    }

}
