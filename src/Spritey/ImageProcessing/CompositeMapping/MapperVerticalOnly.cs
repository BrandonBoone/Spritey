namespace Spritey.ImageProcessing.CompositeMapping
{
    using System.Collections.Generic;

    /// <summary>
    /// Maps images vertically
    /// </summary>
    /// <typeparam name="T">A class that implements <see cref="ISprite"/></typeparam>
    internal class MapperVerticalOnly<T> : IMapper<T>
        where T : class, ISprite, new()
    {
        /// <summary>
        /// Produces a mapping where all images are placed vertically.
        /// </summary>
        /// <param name="images">The images to map</param>
        /// <returns>An instance of <see cref="ISprite"/></returns>
        public T Mapping(IEnumerable<IImageInfo> images)
        {
            T spriteInfo = new T();
            int yOffset = 0;

            foreach (IImageInfo image in images)
            {
                MappedImageInfo imageLocation = new MappedImageInfo(0, yOffset, image);
                spriteInfo.AddMappedImage(imageLocation);
                yOffset += image.Height;
            }

            return spriteInfo;
        }
    }
}
