namespace Spritey.ImageProcessing.Sprites
{
    using System.IO;
    using SixLabors.ImageSharp;
    using SixLabors.ImageSharp.Formats.Gif;
    using SixLabors.ImageSharp.Formats.Png;
    using SixLabors.ImageSharp.PixelFormats;
    using SixLabors.ImageSharp.Processing;
    using Spritey.ImageProcessing.CompositeMapping;

    /// <summary>
    /// The supported output formats of the Sprite image
    /// </summary>
    public enum SpriteFormat
    {
        /// <summary>
        /// GIF (Graphics Interchange Format)
        /// </summary>
        Gif,

        /// <summary>
        /// PNG (Portable Network Graphics)
        /// </summary>
        Png
    }

    /// <summary>
    /// Static methods that assist in the generation of the Sprite image.
    /// </summary>
    public static class SpriteGenerator
    {
        /// <summary>
        /// Generates a GIF Bitmap composite image based off the Sprite blueprint it receives
        /// </summary>
        /// <param name="sprite">A blueprint detailing the layout of the Sprite Image</param>
        /// <returns>A Gif Bitmap composite image</returns>
        public static Image<Rgba32> ComposeSpriteImage(ISprite sprite)
        {
            var image = new Image<Rgba32>(sprite.Width, sprite.Height);

            foreach (IMappedImageInfo minfo in sprite.MappedImages)
            {
                Image<Rgba32> img = ((ImageInfo)minfo.ImageInfo).Img;

                image.Mutate(i => i.DrawImage(GraphicsOptions.Default, img, new SixLabors.Primitives.Point(minfo.X, minfo.Y)));
            }

            return image;
        }

        /// <summary>
        /// Converts composite Sprite image to a GIF <see cref="MemoryStream"/>. Sets the transparent color.
        /// </summary>
        /// <param name="image">The image to convert</param>
        /// <returns>A <see cref="MemoryStream"/> representing the GIF</returns>
        public static MemoryStream ConvertToGifStream(Image<Rgba32> image)
        {
            var streamIn = new MemoryStream();
            var transparent = new Rgba32(0, 255, 0, 0); // transparent color. BUG here if used in source image. TODO: Find a transparent color not in the Color Pallete?
            image.ReplaceTransparentColor(transparent);
            image.Save(streamIn, new GifEncoder() { ColorTableMode = GifColorTableMode.Global, Quantizer = KnownQuantizers.Wu });
            streamIn.Seek(0, SeekOrigin.Begin);
            return Utilities.MakeTransparentGif(streamIn, transparent);
        }

        /// <summary>
        /// Converts composite Sprite image to a PNG <see cref="MemoryStream"/>.
        /// </summary>
        /// <param name="image">The image to convert</param>
        /// <returns>A <see cref="MemoryStream"/> representing the PNG</returns>
        public static MemoryStream ConvertToPngStream(Image<Rgba32> image)
        {
            var streamIn = new MemoryStream();
            image.Save(streamIn, new PngEncoder());
            streamIn.Seek(0, SeekOrigin.Begin);
            return streamIn;
        }
    }
}