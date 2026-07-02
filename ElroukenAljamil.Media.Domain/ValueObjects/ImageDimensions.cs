using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElroukenAljamil.Media.Domain.ValueObjects
{
    /// <summary>
    /// Value Object représentant les dimensions d'une image.
    /// </summary>
    public record ImageDimensions
    {
        public int Width { get; }
        public int Height { get; }

        public ImageDimensions(int width, int height)
        {
            if (width <= 0)
                throw new ArgumentException("La largeur doit être positive.", nameof(width));
            if (height <= 0)
                throw new ArgumentException("La hauteur doit être positive.", nameof(height));

            Width = width;
            Height = height;
        }

        public double AspectRatio => (double)Width / Height;
        public bool IsLandscape => Width > Height;
        public bool IsPortrait => Height > Width;
        public bool IsSquare => Width == Height;

        public override string ToString() => $"{Width}x{Height}";
    }
}
