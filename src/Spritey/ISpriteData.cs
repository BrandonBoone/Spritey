namespace Spritey
{
    /// <summary>
    /// POCO object that contains the meta data about a source sprite image and its location with the <see cref="Sprite"/>
    /// </summary>
    public interface ISpriteData
    {
        /// <summary>
        /// Gets the filename of the source image
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the width of the source image in pixels
        /// </summary>
        int Width { get; }

        /// <summary>
        /// Gets the height of the source image in pixels
        /// </summary>
        int Height { get; }

        /// <summary>
        /// Gets the Top position relative to the <see cref="Sprite"/> total height in pixels.
        /// </summary>
        int Top { get; }

        /// <summary>
        /// Gets the Left position relative to teh <see cref="Sprite"/> total width in pixels.
        /// </summary>
        int Left { get; }
    }
}
