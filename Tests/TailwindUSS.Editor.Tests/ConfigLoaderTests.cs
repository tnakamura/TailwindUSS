using System.IO;

namespace TailwindUSS.Editor.Tests
{
    public sealed class ConfigLoaderTests
    {
        [Test]
        public void TryLoad_UsesDefaultConfigWhenConfigFileIsMissing()
        {
            using var scope = new TestProjectScope();

            var succeeded = ConfigLoader.TryLoad(out var config, out var errorMessage, out var usedDefaultConfig);

            Assert.That(succeeded, Is.True);
            Assert.That(errorMessage, Is.Null);
            Assert.That(usedDefaultConfig, Is.True);
            Assert.That(config.inputGlobs, Is.EqualTo(new[] { "Assets/**/*.uxml" }));
            Assert.That(config.outputUssPath, Is.EqualTo("Assets/Generated/TailwindUSS.generated.uss"));
            Assert.That(ConfigLoader.GetConfigPath(), Is.EqualTo(Path.Combine(scope.RootPath, ConfigLoader.FileName)));
        }

        [Test]
        public void TryLoad_NormalizesMissingValues()
        {
            using var scope = new TestProjectScope();
            scope.WriteProjectFile(ConfigLoader.FileName, "{\"inputGlobs\":[],\"outputUssPath\":\"\"}");

            var succeeded = ConfigLoader.TryLoad(out var config, out var errorMessage, out var usedDefaultConfig);

            Assert.That(succeeded, Is.True);
            Assert.That(errorMessage, Is.Null);
            Assert.That(usedDefaultConfig, Is.False);
            Assert.That(config.inputGlobs, Is.EqualTo(new[] { "Assets/**/*.uxml" }));
            Assert.That(config.outputUssPath, Is.EqualTo("Assets/Generated/TailwindUSS.generated.uss"));
        }

        [Test]
        public void TryLoad_ReturnsErrorForInvalidJson()
        {
            using var scope = new TestProjectScope();
            scope.WriteProjectFile(ConfigLoader.FileName, "{ invalid json }");

            var succeeded = ConfigLoader.TryLoad(out var config, out var errorMessage, out var usedDefaultConfig);

            Assert.That(succeeded, Is.False);
            Assert.That(config, Is.Null);
            Assert.That(errorMessage, Is.Not.Null.And.Not.Empty);
            Assert.That(usedDefaultConfig, Is.False);
        }

        [Test]
        public void WriteDefaultConfig_WritesJsonFile()
        {
            using var scope = new TestProjectScope();

            ConfigLoader.WriteDefaultConfig();

            var writtenJson = File.ReadAllText(Path.Combine(scope.RootPath, ConfigLoader.FileName));
            Assert.That(writtenJson, Does.Contain("\"inputGlobs\""));
            Assert.That(writtenJson, Does.Contain("\"outputUssPath\""));
            Assert.That(writtenJson, Does.EndWith(Environment.NewLine));
        }
    }
}
