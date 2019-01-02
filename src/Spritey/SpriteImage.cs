namespace Spritey
{
    using System;
    using System.IO;
    using Spritey.ImageProcessing;

    /// <summary>
    /// Holds the Sprite image as a <see cref="MemoryStream"/> and a Base64 encoded string
    /// </summary>
    public abstract class SpriteImage : ISpriteImage
    {
        private readonly MemoryStream imgStream;

        /// <summary>
        /// Initializes a new instance of the <see cref="SpriteImage"/> class.
        /// </summary>
        /// <param name="stream">A <see cref="MemoryStream"/> that contains the sprite image</param>
        public SpriteImage(MemoryStream stream)
        {
            this.imgStream = stream;
            this.AsBase64 = Utilities.ConvertToBase64(stream);
            this.Length = Utilities.GetByteCount(this.AsBase64);
        }

        /// <summary>
        /// Gets or sets the Base64 encoded version of the Sprite
        /// </summary>
        public string AsBase64 { get; protected set; }

        /// <summary>
        /// Gets or sets the ByteCount of the Base64 encoded Sprite
        /// </summary>
        public int Length { get; protected set; }

        /// <summary>
        /// Saves the sprite as a file.
        /// </summary>
        /// <param name="path">The file path</param>
        public void Save(string path)
        {
            using (FileStream output = File.OpenWrite(path))
            {
                this.imgStream.Seek(0, SeekOrigin.Begin);
                this.imgStream.WriteTo(output);
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if (this.imgStream != null)
            {
                this.imgStream.Dispose();
            }
        }
    }
}
