namespace Spritey.ImageProcessing.Sprites
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using SixLabors.ImageSharp;
    using SixLabors.ImageSharp.PixelFormats;

    /// <summary>
    /// ImageInfo class
    /// </summary>
    public class ImageInfo : CompositeMapping.IImageInfo, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImageInfo"/> class.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <param name="name">The name of the image. This will be used as the CSS class name if you generate a CSS file.</param>
        /// <remarks>
        /// Stores data about an image you with to merge into a Sprite.
        /// </remarks>
        public ImageInfo(string filename, string name)
        {
            this.Filename = filename;

            this.Img = Image.Load(filename);
            this.Width = this.Img.Width;
            this.Height = this.Img.Height;
            this.Name = Path.GetFileNameWithoutExtension(filename);

            this.IsAnimated = this.Img.Frames.Count > 1;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageInfo"/> class.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <remarks>
        /// Stores data about an image you with to merge into a Sprite.
        /// </remarks>
        public ImageInfo(string filename)
            : this(filename, Path.GetFileNameWithoutExtension(filename))
        {
        }

        /// <summary>
        /// Gets the Width in pixels of the image
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// Gets the Height in pixels of the image
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// Gets the name of the image (this will be used as the CSS class name) if you generate a CSS file
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the image
        /// </summary>
        public Image<Rgba32> Img { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is animated.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is animated; otherwise, <c>false</c>.
        /// </value>
        public bool IsAnimated { get; private set; }

        /// <summary>
        /// Gets the filename.
        /// </summary>
        public string Filename { get; private set; }

        /// <summary>
        /// Gets a list of static ImageInfo objects from the specified image directory path.
        /// </summary>
        /// <param name="sImageDirectoryPath">The image directory path.</param>
        /// <returns>The list of <see cref="ImageInfo"/> from the specified image directory path</returns>
        public static IEnumerable<ImageInfo> GetStaticFromImageDirectory(string sImageDirectoryPath)
        {
            IList<ImageInfo> list = GetFromImageDirectory(sImageDirectoryPath);

            for (int i = list.Count - 1; i >= 0; --i)
            {
                if (list[i].IsAnimated)
                {
                    list[i].Dispose();
                    list.RemoveAt(i);
                }
            }

            return list;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Img.Dispose();
        }

        /// <summary>
        /// Gets a list of filenames from the specified image directory path.
        /// </summary>
        /// <param name="sImageDirectoryPath">The image directory path.</param>
        /// <returns>The list of filenames from the specified directory path</returns>
        private static List<string> GetFilenamesFromImageDirectory(string sImageDirectoryPath)
        {
            var list = new List<string>();

            if (Directory.Exists(sImageDirectoryPath))
            {
                list = Directory.GetFiles(sImageDirectoryPath, "*.*", SearchOption.TopDirectoryOnly).
                    Where(s =>
                        s.EndsWith(".gif", System.StringComparison.OrdinalIgnoreCase)
                        || s.EndsWith(".jpg", System.StringComparison.OrdinalIgnoreCase)
                        || s.EndsWith(".png", System.StringComparison.OrdinalIgnoreCase)
                        || s.EndsWith(".bmp", System.StringComparison.OrdinalIgnoreCase)
                        || s.EndsWith(".ico", System.StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            return list;
        }

        /// <summary>
        /// Gets a list of ImageInfo objects from the specified image directory path.
        /// </summary>
        /// <param name="sImageDirectoryPath">The image directory path.</param>
        /// <returns>The list of <see cref="ImageInfo"/> from the specified image directory path</returns>
        private static IList<ImageInfo> GetFromImageDirectory(string sImageDirectoryPath)
        {
            var list = new List<ImageInfo>();

            if (Directory.Exists(sImageDirectoryPath))
            {
                list = GetFilenamesFromImageDirectory(sImageDirectoryPath)
                    .ConvertAll<ImageInfo>(x => new ImageInfo(x));
            }

            return list;
        }
    }
}