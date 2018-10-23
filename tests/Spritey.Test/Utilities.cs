using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;
using Microsoft.DotNet.PlatformAbstractions;

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
        public static bool FilesAreEqual(FileInfo first, FileInfo second)
        {
            if (first.Length != second.Length)
                return false;

            if (string.Equals(first.FullName, second.FullName, StringComparison.OrdinalIgnoreCase))
                return true;

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
    }
}
