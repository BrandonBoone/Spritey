namespace Spritey
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Spritey.ImageProcessing.CompositeMapping;
    using Spritey.ImageProcessing.Sprites;

    /// <summary>
    /// Processes a directory of images into a composite Sprite image. Outputs CSS and supporting images.
    /// </summary>
    public class Sprite : IDisposable
    {
        private readonly HashSet<char> invalidFileNameChars = new HashSet<char>(Path.GetInvalidFileNameChars());

        /// <summary>
        /// Initializes a new instance of the <see cref="Sprite"/> class.
        /// </summary>
        /// <param name="directory">The directory of images to process</param>
        public Sprite(string directory)
        {
            using (SpriteBlueprint blueprint = SpriteBlueprint.GetFromImageDirectory(directory))
            {
                this.Init(blueprint);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Sprite"/> class.
        /// </summary>
        /// <param name="blueprint">The <see cref="SpriteBlueprint"/> of pre-processed images</param>
        public Sprite(SpriteBlueprint blueprint)
            => this.Init(blueprint);

        /// <summary>
        /// Gets the width of the composite image in pixels
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// Gets the height of the compisite image in pixels
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// Gets a generated identifier for the image. Used incase no name is provided on save
        /// </summary>
        public Guid Id { get; private set; } = Guid.NewGuid();

        /// <summary>
        /// Gets collection of <see cref="SpriteData"/>.
        /// </summary>
        public List<SpriteData> ImageData { get; private set; } = new List<SpriteData>();

        /// <summary>
        /// Gets the generated Png version of the Sprite
        /// </summary>
        public SpriteImage Png { get; private set; }

        /// <summary>
        /// Gets the generated Gif version of the Sprite
        /// </summary>
        public SpriteImage Gif { get; private set; }

        /// <summary>
        /// Save the sprite in  <see cref="SpriteFormat.Gif"/> and <see cref="SpriteFormat.Png"/> embedded and reference formats
        /// </summary>
        /// <param name="name">The name to use when creating sprite artifacts (css, gif, and png files)</param>
        /// <param name="directory">The file directory to output the sprite artifacts</param>
        public void Save(string name, string directory)
        {
            this.Save(name, directory, true, SpriteFormat.Gif);
            this.Save(name, directory, true, SpriteFormat.Png);
            this.Save(name, directory, false, SpriteFormat.Gif);
            this.Save(name, directory, false, SpriteFormat.Png);
        }

        /// <summary>
        /// Save the sprite in both <see cref="SpriteFormat.Gif"/> and <see cref="SpriteFormat.Png"/>
        /// </summary>
        /// <param name="name">The name to use when creating sprite artifacts (css, gif, and png files)</param>
        /// <param name="directory">The file directory to output the sprite artifacts</param>
        /// <param name="isSpriteEmbedded">true, the Sprite is Base64 encoded as a data uri within the css file. false the sprite is saved as a referenced image file.</param>
        public void Save(string name, string directory, bool isSpriteEmbedded)
        {
            this.Save(name, directory, isSpriteEmbedded, SpriteFormat.Gif);
            this.Save(name, directory, isSpriteEmbedded, SpriteFormat.Png);
        }

        /// <summary>
        /// Save the sprite in a specific format
        /// </summary>
        /// <param name="name">The name to use when creating sprite artifacts (css, gif, and png files)</param>
        /// <param name="directory">The file directory to output the sprite artifacts</param>
        /// <param name="isSpriteEmbedded">true, the Sprite is Base64 encoded as a data uri within the css file. false the sprite is saved as a referenced image file.</param>
        /// <param name="fmt"><see cref="SpriteFormat.Gif"/> or <see cref="SpriteFormat.Png"/></param>
        public void Save(string name, string directory, bool isSpriteEmbedded, SpriteFormat fmt)
        {
            name = this.CleanFileName(name ?? this.Id.ToString());
            string cssFileName = Path.Combine(directory, $"{name}_{(fmt == SpriteFormat.Gif ? "g" : "p")}{(isSpriteEmbedded ? "e" : string.Empty)}.css");
            if (!isSpriteEmbedded)
            {
                if (fmt == SpriteFormat.Gif)
                {
                    this.Gif.Save(Path.Combine(directory, $"{name}.gif"));
                }
                else
                {
                    this.Png.Save(Path.Combine(directory, $"{name}.png"));
                }

                File.AppendAllText(cssFileName, SpriteCss.GetReferencedSpriteCSS(this, name, fmt));
            }
            else
            {
                File.AppendAllText(cssFileName, SpriteCss.GetEmbeddedSpriteCSS(this, fmt));
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if (this.Gif != null)
            {
                this.Gif.Dispose();
            }

            if (this.Png != null)
            {
                this.Png.Dispose();
            }
        }

        // https://stackoverflow.com/a/18917438/402706
        // Answered: [csells](https://stackoverflow.com/users/185286/csells)
        // Asked: [Martin Doms](https://stackoverflow.com/users/13877/martin-doms)
        private string CleanFileName(string name)
            => new string(name.Select(c => this.invalidFileNameChars.Contains(c) ? '_' : c).ToArray());

        // https://stackoverflow.com/a/10938592/402706
        // Answered: [Channs](https://stackoverflow.com/users/1271728/channs)
        // Asked: [TruMan1](https://stackoverflow.com/users/235334/truman1)
        private string CleanCSSName(string name)
            => new Regex(@"[!""#$%&'()\*\+,\./:;<=>\?@\[\\\]^`{\|}~ ]").Replace(name, string.Empty);

        private void Init(SpriteBlueprint blueprint)
        {
            this.Width = blueprint.Width;
            this.Height = blueprint.Height;

            using (var image = SpriteGenerator.ComposeSpriteImage(blueprint))
            {
                this.Png = new SpritePng(image);
                this.Gif = new SpriteGif(image);
            }

            foreach (IMappedImageInfo minfo in blueprint.MappedImages)
            {
                ImageInfo info = (ImageInfo)minfo.ImageInfo;

                this.ImageData.Add(new SpriteData()
                {
                    Name = this.CleanCSSName(info.Name),
                    Width = info.Width,
                    Height = info.Height,
                    Top = minfo.Y,
                    Left = minfo.X
                });
            }
        }
    }
}