using System;
using Xunit;
using Spritey;
using System.Reflection;
using Spritey.ImageProcessing.Sprites;
using Spritey.ImageProcessing.CompositeMapping;

namespace Spritey.Test
{
    public class SpriteBlueprintTest
    {
        [Fact]
        public void Should_ContainCorrectImages_When_BlueprintIsCalled()
        {
            string path = Utilities.GetTestDataFolder("TestSet1");
            using(ISpriteBlueprint blueprint = SpriteBlueprint.GetFromImageDirectory(path))
            {
                Assert.Equal(32, blueprint.Height);
                Assert.Equal(160, blueprint.Width);
                Assert.Equal(7, blueprint.MappedImages.Count);
            }
        }
    }
}
