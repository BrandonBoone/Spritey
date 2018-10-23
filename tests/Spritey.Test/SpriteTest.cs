using Spritey.ImageProcessing.Sprites;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using System.IO;

namespace Spritey.Test.ImageProcessing.Sprites
{
    public class SpriteTest
    {
        const string RESULTS_DIR = "results";

        [Fact]
        public void Sprite_ConstructorWithBlueprint_ImageProcessedCorrectly()
        {
            string path = Utilities.GetTestDataFolder("TestSet1");
            using (SpriteBlueprint blueprint = SpriteBlueprint.GetFromImageDirectory(path))
            using (Sprite sprite = new Sprite(blueprint))
            {
                Assert.Equal(32, sprite.Height);
                Assert.Equal(160, sprite.Width);
                Assert.Equal(7, sprite.ImageData.Count);
            }
        }

        [Fact]
        public void Sprite_ConstructorWithPath_ImageProcessedCorrectly()
        {
            string path = Utilities.GetTestDataFolder("TestSet1");
            using (Sprite sprite = new Sprite(path))
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
                string comparisonPath = Utilities.GetTestDataFolder("SnapShots");

                using (Sprite sprite = new Sprite(path))
                {
                    resultsDir = Path.Join(path, RESULTS_DIR);
                    CreateDirectoryIfNotExists(resultsDir);

                    string filePath = Path.Join(resultsDir, "sprite.png");
                    sprite.Png.Save(filePath);

                    string comparisonFile = Path.Join(comparisonPath, testSet, "sprite.png");
                    Assert.True(Utilities.FilesAreEqual(new FileInfo(comparisonFile), new FileInfo(filePath)));
                }
            }
            finally
            {
                RemoveDirectoryIfExists(resultsDir);
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
                string comparisonPath = Utilities.GetTestDataFolder("SnapShots");

                using (Sprite sprite = new Sprite(path))
                {
                    resultsDir = Path.Join(path, RESULTS_DIR);
                    CreateDirectoryIfNotExists(resultsDir);

                    string filePath = Path.Join(resultsDir, "sprite.gif");
                    sprite.Gif.Save(filePath);

                    string comparisonFile = Path.Join(comparisonPath, testSet, "sprite.gif");
                    Assert.True(Utilities.FilesAreEqual(new FileInfo(comparisonFile), new FileInfo(filePath)));
                }
            }
            finally
            {
                RemoveDirectoryIfExists(resultsDir);
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
                string comparisonPath = Utilities.GetTestDataFolder("SnapShots");

                using (Sprite sprite = new Sprite(path))
                {
                    
                    resultsDir = Path.Join(path, RESULTS_DIR);
                    CreateDirectoryIfNotExists(resultsDir);

                    sprite.Save("sprite", resultsDir);

                    foreach(string file in Directory.GetFiles(resultsDir))
                    {
                        string comparisonFile = Path.Join(comparisonPath, testSet, Path.GetFileName(file));
                        Assert.True(Utilities.FilesAreEqual(new FileInfo(comparisonFile), new FileInfo(file)));
                    }
                }
            }
            finally
            {
                RemoveDirectoryIfExists(resultsDir);
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
