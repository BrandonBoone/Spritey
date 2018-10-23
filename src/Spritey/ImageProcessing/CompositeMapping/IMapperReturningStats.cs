namespace Spritey.ImageProcessing.CompositeMapping
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines an iterative mapper. That is, a mapper that generates a number of candidate sprites
    /// and picks the best one.
    /// </summary>
    /// <typeparam name="T">A class that implements <see cref="ISprite"/></typeparam>
    internal interface IMapperReturningStats<T> : IMapper<T>
        where T : class, ISprite, new()
    {
        /// <summary>
        /// Version of IMapper.Mapping. See IMapper.
        /// </summary>
        /// <param name="images">Same as for IMapper.Mapping</param>
        /// <param name="mapperStats">
        /// The method will fill the properties of this statistics object.
        /// Set to null if you don't want statistics.
        /// </param>
        /// <returns>
        /// A SpriteInfo object. This describes the locations of the images within the sprite,
        /// and the dimensions of the sprite.
        /// </returns>
        T Mapping(IEnumerable<IImageInfo> images, IMapperStats mapperStats);
    }
}
