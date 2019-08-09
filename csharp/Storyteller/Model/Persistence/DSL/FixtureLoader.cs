using System;
using System.IO;
using System.Linq;
using Baseline;

namespace Storyteller.Model.Persistence.DSL
{
    public class FixtureLoader
    {
        public static string SelectFixturePath(string baseDirectory)
        {
            var specPath = baseDirectory.AppendPath("Fixtures");
            if (Directory.Exists(specPath)) return specPath;

            return specPath;
        }

        public static FixtureLibrary LoadFromPath(string path)
        {
            if (!Directory.Exists(path))
            {
                Console.WriteLine("Created directory " + path);
                Directory.CreateDirectory(path);
            }

            var fixturePaths = Directory.GetFiles(path)
                .Where(file =>
                    Path.GetExtension(file).Equals(
                        ".md",
                        StringComparison.OrdinalIgnoreCase));

            var fixtures = fixturePaths.Select(FixtureReader.ReadFromFile).ToArray();

            return FixtureLibrary.From(fixtures);
        }
    }
}
