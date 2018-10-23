namespace Spritey
{
    /// <summary>
    /// POCO object that contains the meta data about a source sprite image and its location with the <see cref="Sprite"/>
    /// </summary>
    public class SpriteData
    {
        /// <summary>
        /// Gets the filename of the source image
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Gets the width of the source image in pixels
        /// </summary>
        public int Width { get; internal set; }

        /// <summary>
        /// Gets the height of the source image in pixels
        /// </summary>
        public int Height { get; internal set; }

        /// <summary>
        /// Gets the Top position relative to the <see cref="Sprite"/> total height in pixels.
        /// </summary>
        public int Top { get; internal set; }

        /// <summary>
        /// Gets the Left position relative to teh <see cref="Sprite"/> total width in pixels.
        /// </summary>
        public int Left { get; internal set; }
    }
}
