using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElroukenAljamil.Media.Domain.ValueObjects
{
    /// <summary>
    /// Value Object contenant les chemins vers les différentes variantes d'une image.
    /// </summary>
    public record MediaVariants
    {
        public string? ThumbnailPath { get; init; }   // 150px
        public string? MediumPath { get; init; }      // 600px
        public string? LargePath { get; init; }       // 1200px
        public string? WebPPath { get; init; }        // Original converti en WebP

        public static MediaVariants Empty() => new()
        {
            ThumbnailPath = null,
            MediumPath = null,
            LargePath = null,
            WebPPath = null
        };

        public bool IsComplete =>
            ThumbnailPath is not null &&
            MediumPath is not null &&
            LargePath is not null;
    }

}
