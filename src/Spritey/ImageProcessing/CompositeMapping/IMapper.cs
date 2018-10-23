namespace Spritey.ImageProcessing.CompositeMapping
{
    using System.Collections.Generic;

    /// <summary>
    /// An IMapper takes a series of images, and figures out how these could be combined in a sprite.
    /// It returns the dimensions that the sprite will have, and the locations of each image within that sprite.
    ///
    /// This object does not create the sprite image itself. It only figures out how it needs to be constructed.
    /// Code taken From: http://www.codeproject.com/Articles/210979/Fast-optimizing-rectangle-packing-algorithm-for-bu
    /// Protected for fair use under the - The Code Project Open License (CPOL) http://www.codeproject.com/info/cpol10.aspx
    /// </summary>
    /// <typeparam name="T">A class that implements <see cref="ISprite"/></typeparam>
    public interface IMapper<T>
        where T : class, ISprite, new()
    {
        /// <summary>
        /// Works out how to map a series of images into a sprite.
        /// </summary>
        /// <param name="images">
        /// The list of images to place into the sprite.
        /// </param>
        /// <returns>
        /// A SpriteInfo object. This describes the locations of the images within the sprite,
        /// and the dimensions of the sprite.
        /// </returns>
        T Mapping(IEnumerable<IImageInfo> images);
    }
}
