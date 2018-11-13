using Spritey.ImageProcessing.Sprites;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Xunit;
using Xunit.Abstractions;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Spritey.ImageProcessing.CompositeMapping;

namespace Spritey.Test.ImageProcessing.Sprites
{
    public class SpriteTest
    {
        const string RESULTS_DIR = "results";

        private readonly ITestOutputHelper output;

        public SpriteTest(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void Sprite_ConstructorWithBlueprint_ImageProcessedCorrectly()
        {
            string path = Utilities.GetTestDataFolder("TestSet1");
            this.output.WriteLine("path is {0}", path);

            using (var blueprint = SpriteBlueprint.GetFromImageDirectory(path))
            using (var sprite = new Sprite(blueprint))
            {
                string imagesToProcess = sprite.ImageData.Aggregate("", (prev, next) => $"{(prev != "" ? $"{prev}," : "")}{next.Name}");
                this.output.WriteLine("processing Images: {0}", imagesToProcess);
                Assert.Equal(32, sprite.Height);
                Assert.Equal(160, sprite.Width);
                Assert.Equal(7, sprite.ImageData.Count);
            }
        }

        [Fact]
        public void Sprite_ConstructorWithPath_ImageProcessedCorrectly()
        {
            string path = Utilities.GetTestDataFolder("TestSet1");
            this.output.WriteLine("path is {0}", path);

            using (var sprite = new Sprite(path))
            {
                Assert.Equal(32, sprite.Height);
                Assert.Equal(160, sprite.Width);
                Assert.Equal(7, sprite.ImageData.Count);
            }
        }

        [Theory]
        [InlineData("TestSet1")]
        [InlineData("TestSet2")]
        [InlineData("TestSet3")]
        [InlineData("TestSet4")]
        public void Sprite_PngSave_PngOutputIsCorrect(string testSet)
        {
            string resultsDir = "";
            try
            {
                string path = Utilities.GetTestDataFolder(testSet);
                this.output.WriteLine("path is {0}", path);

                string comparisonPath = Utilities.GetTestDataFolder("SnapShots");
                this.output.WriteLine("comparisonPath is {0}", comparisonPath);

                using (var sprite = new Sprite(path))
                {
                    string imagesToProcess = sprite.ImageData.Aggregate("", (prev, next) => $"{(prev != "" ? $"{prev}," : "")}{next.Name}");
                    this.output.WriteLine("processing Images: {0}", imagesToProcess);

                    resultsDir = Path.Combine(path, RESULTS_DIR);
                    this.CreateDirectoryIfNotExists(resultsDir);

                    string filePath = Path.Combine(resultsDir, "sprite.png");
                    sprite.Png.Save(filePath);

                    string comparisonFile = Path.Combine(comparisonPath, testSet, "sprite.png");

                    var actualImage = Image.Load(filePath);
                    var expectedImage = Image.Load(comparisonFile);
                    Assert.True(Utilities.ImagesAreEqual(expectedImage, actualImage, this.output));
                    // Assert.True(Utilities.FilesAreEqual(new FileInfo(comparisonFile), new FileInfo(filePath), this.output));
                }
            }
            finally
            {
                this.RemoveDirectoryIfExists(resultsDir);
            }
        }

        [Theory]
        [InlineData("TestSet1")]
        [InlineData("TestSet2")]
        [InlineData("TestSet3")]
        [InlineData("TestSet4")]
        public void Sprite_GifSave_GifOutputIsCorrect(string testSet)
        {
            string resultsDir = "";
            try
            {
                string path = Utilities.GetTestDataFolder(testSet);
                this.output.WriteLine("path is {0}", path);

                string comparisonPath = Utilities.GetTestDataFolder("SnapShots");
                this.output.WriteLine("comparisonPath is {0}", comparisonPath);

                using (var sprite = new Sprite(path))
                {
                    string imagesToProcess = sprite.ImageData.Aggregate("", (prev, next) => $"{(prev != "" ? $"{prev}," : "")}{next.Name}");
                    this.output.WriteLine("processing Images: {0}", imagesToProcess);

                    resultsDir = Path.Combine(path, RESULTS_DIR);
                    this.CreateDirectoryIfNotExists(resultsDir);

                    string filePath = Path.Combine(resultsDir, "sprite.gif");
                    sprite.Gif.Save(filePath);

                    string comparisonFile = Path.Combine(comparisonPath, testSet, "sprite.gif");

                    var actualImage = Image.Load(filePath);
                    var expectedImage = Image.Load(comparisonFile);
                    Assert.True(Utilities.ImagesAreEqual(expectedImage, actualImage, this.output));

                    // Assert.True(Utilities.FilesAreEqual(new FileInfo(comparisonFile), new FileInfo(filePath), this.output));
                }
            }
            finally
            {
                this.RemoveDirectoryIfExists(resultsDir);
            }
        }

        [Theory]
        [InlineData("TestSet1")]
        [InlineData("TestSet2")]
        [InlineData("TestSet3")]
        [InlineData("TestSet4")]
        public void Sprite_SaveAll_OutputIsCorrect(string testSet)
        {
            string resultsDir = "";
            try
            {
                string path = Utilities.GetTestDataFolder(testSet);
                this.output.WriteLine("path is {0}", path);
                
                string comparisonPath = Utilities.GetTestDataFolder("SnapShots");
                this.output.WriteLine("comparisonPath is {0}", comparisonPath);

                using (var sprite = new Sprite(path))
                {
                    string imagesToProcess = sprite.ImageData.Aggregate("", (prev, next) => $"{(prev != "" ? $"{prev}," : "")}{next.Name}");
                    this.output.WriteLine("processing Images: {0}", imagesToProcess);

                    resultsDir = Path.Combine(path, RESULTS_DIR);
                    this.CreateDirectoryIfNotExists(resultsDir);

                    sprite.Save("sprite", resultsDir);

                    foreach(string file in Directory.GetFiles(resultsDir))
                    {
                        string ext = Path.GetExtension(file);
                        string comparisonFile = Path.Combine(comparisonPath, testSet, Path.GetFileName(file));
                        if (ext == ".css")
                        {
                            Assert.True(Utilities.FilesAreEqual(new FileInfo(comparisonFile), new FileInfo(file), this.output));
                        }
                        else
                        {
                            var actualImage = Image.Load(file);
                            var expectedImage = Image.Load(comparisonFile);
                            Assert.True(Utilities.ImagesAreEqual(expectedImage, actualImage, this.output));
                        }
                        
                        // Assert.True(Utilities.FilesAreEqual(new FileInfo(comparisonFile), new FileInfo(file), this.output));
                    }
                }
            }
            finally
            {
                this.RemoveDirectoryIfExists(resultsDir);
            }
        }

        private void CreateDirectoryIfNotExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        private void RemoveDirectoryIfExists(string path)
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
        }
    }
}
