using FluentValidation;

namespace ElroukenAljamil.Media.Application.Commands.UploadMedia
{
    public class UploadMediaCommandValidator : AbstractValidator<UploadMediaCommand>
    {
        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".webp", ".gif", ".avif" };
        private const long MaxFileSize = 10 * 1024 * 1024; // 10 MB

        public UploadMediaCommandValidator()
        {
            RuleFor(x => x.File)
                .NotNull().WithMessage("Le fichier est obligatoire.");

            RuleFor(x => x.File.Length)
                .GreaterThan(0).WithMessage("Le fichier est vide.")
                .LessThanOrEqualTo(MaxFileSize).WithMessage("La taille maximale est de 10 MB.");

            RuleFor(x => x.File.FileName)
                .Must(HaveAllowedExtension)
                .WithMessage($"Extensions autorisées : {string.Join(", ", AllowedExtensions)}");
        }

        private static bool HaveAllowedExtension(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return false;
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            return AllowedExtensions.Contains(extension);
        }
    }


}
