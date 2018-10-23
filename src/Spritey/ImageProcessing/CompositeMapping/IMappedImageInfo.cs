namespace Spritey.ImageProcessing.CompositeMapping
{
    /// <summary>
    /// Defines an image that has been mapped to a specific location, for example within a sprite.
    /// </summary>
    public interface IMappedImageInfo
    {
        /// <summary>
        /// Gets the top position in pixels
        /// </summary>
        int X { get; }

        /// <summary>
        /// Gets the left position in pixels
        /// </summary>
        int Y { get; }

        /// <summary>
        /// Gets the image
        /// </summary>
        IImageInfo ImageInfo { get; }
    }
}
