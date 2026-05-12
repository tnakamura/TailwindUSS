using System.Collections.Generic;
using System.IO;

namespace TailwindUSS.Editor.Tests
{
    /// <summary>
    /// Represents the config loader tests.
    /// </summary>
    public sealed class ConfigLoaderTests
    {
        /// <summary>
        /// Tests that try load uses default config when config file is missing.
        /// </summary>
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
            Assert.That(config.theme.colors["blue-500"], Is.EqualTo("#3B82F6"));
            Assert.That(config.theme.spacing["4"], Is.EqualTo("16px"));
            Assert.That(config.theme.fontSizes["xl"], Is.EqualTo("20px"));
            Assert.That(ConfigLoader.GetConfigPath(), Is.EqualTo(Path.Combine(scope.RootPath, ConfigLoader.FileName)));
        }

        /// <summary>
        /// Tests that try load normalizes missing values.
        /// </summary>
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
            Assert.That(config.theme.colors["blue-500"], Is.EqualTo("#3B82F6"));
        }

        /// <summary>
        /// Tests that try load merges theme overrides with built in defaults.
        /// </summary>
        [Test]
        public void TryLoad_MergesThemeOverridesWithBuiltInDefaults()
        {
            using var scope = new TestProjectScope();
            scope.WriteProjectFile(ConfigLoader.FileName, """
            {
              "theme": {
                "colors": { "brand": "#112233", "blue-500": "#123456" },
                "spacing": { "16": "64px" },
                "fontSizes": { "display": "42px" },
                "fonts": { "sans": "Fonts/Inter-Regular" },
                "backgroundImages": { "hero": "Images/Hero" }
              }
            }
            """);

            var succeeded = ConfigLoader.TryLoad(out var config, out var errorMessage, out var usedDefaultConfig);

            Assert.That(succeeded, Is.True);
            Assert.That(errorMessage, Is.Null);
            Assert.That(usedDefaultConfig, Is.False);
            Assert.That(config.theme.colors["brand"], Is.EqualTo("#112233"));
            Assert.That(config.theme.colors["blue-500"], Is.EqualTo("#123456"));
            Assert.That(config.theme.spacing["4"], Is.EqualTo("16px"));
            Assert.That(config.theme.spacing["16"], Is.EqualTo("64px"));
            Assert.That(config.theme.fontSizes["display"], Is.EqualTo("42px"));
            Assert.That(config.theme.fontSizes["sm"], Is.EqualTo("14px"));
            Assert.That(config.theme.fonts["sans"], Is.EqualTo("Fonts/Inter-Regular"));
            Assert.That(config.theme.backgroundImages["hero"], Is.EqualTo("Images/Hero"));
        }

        /// <summary>
        /// Tests that try load returns error for invalid json.
        /// </summary>
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

        /// <summary>
        /// Tests that try load returns error for invalid theme entry.
        /// </summary>
        [Test]
        public void TryLoad_ReturnsErrorForInvalidThemeEntry()
        {
            using var scope = new TestProjectScope();
            scope.WriteProjectFile(ConfigLoader.FileName, """
            {
              "theme": {
                "fonts": { "sans": "" }
              }
            }
            """);

            var succeeded = ConfigLoader.TryLoad(out var config, out var errorMessage, out var usedDefaultConfig);

            Assert.That(succeeded, Is.False);
            Assert.That(config, Is.Null);
            Assert.That(errorMessage, Is.EqualTo("Config section 'theme.fonts' contains an empty value for 'sans'."));
            Assert.That(usedDefaultConfig, Is.False);
        }

        /// <summary>
        /// Tests that write default config writes json file.
        /// </summary>
        [Test]
        public void WriteDefaultConfig_WritesJsonFile()
        {
            using var scope = new TestProjectScope();

            ConfigLoader.WriteDefaultConfig();

            var writtenJson = File.ReadAllText(Path.Combine(scope.RootPath, ConfigLoader.FileName));
            Assert.That(writtenJson, Does.Contain("\"inputGlobs\""));
            Assert.That(writtenJson, Does.Contain("\"outputUssPath\""));
            Assert.That(writtenJson, Does.Not.Contain("\"theme\""));
            Assert.That(writtenJson, Does.EndWith(Environment.NewLine));
        }

        /// <summary>
        /// Tests that try load editable does not merge built in theme defaults.
        /// </summary>
        [Test]
        public void TryLoadEditable_DoesNotMergeBuiltInThemeDefaults()
        {
            using var scope = new TestProjectScope();
            scope.WriteProjectFile(ConfigLoader.FileName, """
            {
              "theme": {
                "colors": { "brand": "#112233" }
              }
            }
            """);

            var succeeded = ConfigLoader.TryLoadEditable(out var config, out var errorMessage, out var fileExists);

            Assert.That(succeeded, Is.True);
            Assert.That(errorMessage, Is.Null);
            Assert.That(fileExists, Is.True);
            Assert.That(config.theme.colors.Keys, Is.EqualTo(new[] { "brand" }));
        }

        /// <summary>
        /// Tests that write config writes provided overrides without built in theme defaults.
        /// </summary>
        [Test]
        public void WriteConfig_WritesProvidedOverridesWithoutBuiltInThemeDefaults()
        {
            using var scope = new TestProjectScope();

            ConfigLoader.WriteConfig(new TailwindUssConfig
            {
                inputGlobs = new[] { "Assets/UI/**/*.uxml" },
                outputUssPath = "Assets/Generated/Custom.uss",
                autoAttachGeneratedUss = true,
                theme = new TailwindUssTheme
                {
                    colors = new Dictionary<string, string> { { "brand", "#112233" } }
                }
            });

            var writtenJson = File.ReadAllText(Path.Combine(scope.RootPath, ConfigLoader.FileName));

            Assert.That(writtenJson, Does.Contain("\"brand\": \"#112233\""));
            Assert.That(writtenJson, Does.Not.Contain("\"blue-500\""));
            Assert.That(writtenJson, Does.Contain("\"autoAttachGeneratedUss\": true"));
        }
    }
}
