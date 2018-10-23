namespace Spritey
{
    using System.IO;
    using SixLabors.ImageSharp;
    using SixLabors.ImageSharp.PixelFormats;
    using Spritey.ImageProcessing.Sprites;

    /// <summary>
    /// Holds the PNG Sprite image as a <see cref="MemoryStream"/> and a Base64 encoded string
    /// </summary>
    public class SpritePng : SpriteImage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SpritePng"/> class.
        /// </summary>
        /// <param name="stream">A <see cref="MemoryStream"/> that contains a PNG image</param>
        public SpritePng(MemoryStream stream)
            : base(stream)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpritePng"/> class.
        /// </summary>
        /// <param name="image">An <see cref="Image{Rgba32}"/> that contains a PNG image</param>
        public SpritePng(Image<Rgba32> image)
            : this(SpriteGenerator.ConvertToPngStream(image))
        {
        }
    }
}
