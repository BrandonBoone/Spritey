namespace Spritey
{
    using System.IO;
    using SixLabors.ImageSharp;
    using SixLabors.ImageSharp.PixelFormats;
    using Spritey.ImageProcessing.Sprites;

    /// <summary>
    /// Holds the GIF Sprite image as a <see cref="MemoryStream"/> and a Base64 encoded string
    /// </summary>
    public class SpriteGif : SpriteImage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SpriteGif"/> class.
        /// </summary>
        /// <param name="stream">A <see cref="MemoryStream"/> that contains a GIF image</param>
        public SpriteGif(MemoryStream stream)
            : base(stream)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpriteGif"/> class.
        /// </summary>
        /// <param name="image">An <see cref="Image{Rgba32}"/> that contains a GIF image</param>
        public SpriteGif(Image<Rgba32> image)
            : this(SpriteGenerator.ConvertToGifStream(image))
        {
        }
    }
}
