using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;
using Xunit;
using Xunit.Abstractions;
using Microsoft.DotNet.PlatformAbstractions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Spritey.Test
{
    public static class Utilities
    {
        // https://stackoverflow.com/a/25573832/402706
        // Answered: [DaniCE](https://stackoverflow.com/users/19046/danice)
        // Asked: [BigBrother](https://stackoverflow.com/users/96630/bigbrother)
        public static string GetTestDataFolder(string testDataFolder)
        {
            string startupPath = ApplicationEnvironment.ApplicationBasePath;
            var pathItems = startupPath.Split(Path.DirectorySeparatorChar);
            var pos = pathItems.Reverse().ToList().FindIndex(x => string.Equals("bin", x));
            string projectPath = String.Join(Path.DirectorySeparatorChar.ToString(), pathItems.Take(pathItems.Length - pos - 1));
            return Path.Combine(projectPath, "TestData", testDataFolder);
        }

        const int BYTES_TO_READ = sizeof(Int64);


        // https://stackoverflow.com/a/1359947/402706
        // Answered: [chsh](https://stackoverflow.com/users/122268/chsh)
        // Asked: [Robinicks](https://stackoverflow.com/users/41021/robinicks)
        public static bool FilesAreEqual(FileInfo first, FileInfo second, ITestOutputHelper output)
        {
            if (first.Length != second.Length)
            {
                output.WriteLine("fist.Length is {0}; second.Length is {1}", first.Length, second.Length);
                return false;
            }

            if (string.Equals(first.FullName, second.FullName, StringComparison.OrdinalIgnoreCase))
            {
                output.WriteLine("fist.FullName is {0}; second.FullName is {1}", first.FullName, second.FullName);
                return true;
            }

            int iterations = (int)Math.Ceiling((double)first.Length / BYTES_TO_READ);

            using (FileStream fs1 = first.OpenRead())
            using (FileStream fs2 = second.OpenRead())
            {
                byte[] one = new byte[BYTES_TO_READ];
                byte[] two = new byte[BYTES_TO_READ];

                for (int i = 0; i < iterations; i++)
                {
                    fs1.Read(one, 0, BYTES_TO_READ);
                    fs2.Read(two, 0, BYTES_TO_READ);

                    if (BitConverter.ToInt64(one, 0) != BitConverter.ToInt64(two, 0))
                        return false;
                }
            }

            return true;
        }

        // https://github.com/SixLabors/ImageSharp/blob/fc4da81123d549eb675e9ef29014cbbec8ab64b5/tests/ImageSharp.Tests/TestUtilities/ImageComparison/ExactImageComparer.cs
        // TODO: See if you can integrate the SixLabors ExactImageComparer class.
        public static bool ImagesAreEqual(
            Image<Rgba32> expected,
            Image<Rgba32> actual)
        {
            if (expected.Size() != actual.Size())
            {
                return false;
            }

            for (int y = 0; y < expected.Height; ++y)
            {
                for (int x = 0; x < expected.Width; ++x)
                {
                    Rgba32 exp = expected[x, y];
                    Rgba32 act = actual[x, y];

                    if (exp != act)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
