namespace Spritey
{
    using System;

    /// <summary>
    /// Holds the Sprite image data
    /// </summary>
    public interface ISpriteImage : IDisposable
    {
        /// <summary>
        /// Gets the Base64 encoded version of the Sprite
        /// </summary>
        string AsBase64 { get; }

        /// <summary>
        /// Gets the ByteCount of the Base64 encoded Sprite
        /// </summary>
        int Length { get; }

        /// <summary>
        /// Saves the sprite as a file.
        /// </summary>
        /// <param name="path">The file path</param>
        void Save(string path);
    }
}
