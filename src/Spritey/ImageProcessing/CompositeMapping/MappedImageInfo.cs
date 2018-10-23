namespace Spritey.ImageProcessing.CompositeMapping
{
    /// <summary>
    /// Defines an image that has been mapped to a specific location, for example within a sprite.
    /// </summary>
    internal class MappedImageInfo : IMappedImageInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MappedImageInfo"/> class.
        /// </summary>
        /// <param name="x">The top position in pixels</param>
        /// <param name="y">The left position in pixels</param>
        /// <param name="imageInfo">The image</param>
        internal MappedImageInfo(int x, int y, IImageInfo imageInfo)
        {
            this.X = x;
            this.Y = y;
            this.ImageInfo = imageInfo;
        }

        /// <inheritdoc/>
        public int X { get; private set; }

        /// <inheritdoc/>
        public int Y { get; private set; }

        /// <inheritdoc/>
        public IImageInfo ImageInfo { get; private set; }
    }
}
