namespace Spritey
{
    /// <summary>
    /// POCO object that contains the meta data about a source sprite image and its location with the <see cref="Sprite"/>
    /// </summary>
    public class SpriteData : ISpriteData
    {
        /// <inheritdoc/>
        public string Name { get; internal set; }

        /// <inheritdoc/>
        public int Width { get; internal set; }

        /// <inheritdoc/>
        public int Height { get; internal set; }

        /// <inheritdoc/>
        public int Top { get; internal set; }

        /// <inheritdoc/>
        public int Left { get; internal set; }
    }
}
