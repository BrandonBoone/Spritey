namespace Spritey.ImageProcessing.CompositeMapping
{
    using System.Collections.Generic;

    /// <summary>
    /// Maps images horizontally
    /// </summary>
    /// <typeparam name="T">A class that implements <see cref="ISprite"/></typeparam>
    internal class MapperHorizontalOnly<T> : IMapper<T>
        where T : class, ISprite, new()
    {
        /// <summary>
        /// Produces a mapping where all images are placed horizontally.
        /// </summary>
        /// <param name="images">The images to map</param>
        /// <returns>An instance of <see cref="ISprite"/></returns>
        public T Mapping(IEnumerable<IImageInfo> images)
        {
            T spriteInfo = new T();
            int xOffset = 0;

            foreach (IImageInfo image in images)
            {
                MappedImageInfo imageLocation = new MappedImageInfo(xOffset, 0, image);
                spriteInfo.AddMappedImage(imageLocation);
                xOffset += image.Width;
            }

            return spriteInfo;
        }
    }
}
