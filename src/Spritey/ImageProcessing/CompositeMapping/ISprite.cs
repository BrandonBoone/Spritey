namespace Spritey.ImageProcessing.CompositeMapping
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents the contents of a sprite image.
    /// </summary>
    public interface ISprite
    {
        /// <summary>
        /// Gets width of the sprite image
        /// </summary>
        int Width { get; }

        /// <summary>
        /// Gets height of the sprite image
        /// </summary>
        int Height { get; }

        /// <summary>
        /// Gets area of the sprite image
        /// </summary>
        int Area { get; }

        /// <summary>
        /// Gets holds the locations of all the individual images within the sprite image.
        /// </summary>
        List<IMappedImageInfo> MappedImages { get; }

        /// <summary>
        /// Adds an image to the SpriteInfo, and updates the width and height of the SpriteInfo.
        /// </summary>
        /// <param name="mappedImage">The image to add</param>
        void AddMappedImage(IMappedImageInfo mappedImage);
    }
}
