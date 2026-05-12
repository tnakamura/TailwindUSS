using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace TailwindUSS.Editor.Tests
{
    /// <summary>
    /// Represents the tailwind uss menu tests.
    /// </summary>
    public sealed class TailwindUssMenuTests
    {
        /// <summary>
        /// Tests that create default config writes config when user confirms overwrite.
        /// </summary>
        [Test]
        public void CreateDefaultConfig_WritesConfigWhenUserConfirmsOverwrite()
        {
            using var scope = new TestProjectScope();
            scope.WriteProjectFile(ConfigLoader.FileName, "{}");

            InvokeCreateDefaultConfig();

            Assert.That(File.ReadAllText(scope.GetProjectPath(ConfigLoader.FileName)), Does.Contain("\"inputGlobs\""));
            Assert.That(EditorUtility.Calls, Has.Count.EqualTo(1));
            Assert.That(AssetDatabase.RefreshCallCount, Is.EqualTo(1));
            Assert.That(Debug.Entries.Last().Message, Does.Contain("default config created"));
        }

        /// <summary>
        /// Tests that create default config does nothing when user cancels overwrite.
        /// </summary>
        [Test]
        public void CreateDefaultConfig_DoesNothingWhenUserCancelsOverwrite()
        {
            using var scope = new TestProjectScope();
            scope.WriteProjectFile(ConfigLoader.FileName, "original");
            EditorUtility.NextDisplayDialogResult = false;

            InvokeCreateDefaultConfig();

            Assert.That(File.ReadAllText(scope.GetProjectPath(ConfigLoader.FileName)), Is.EqualTo("original"));
            Assert.That(EditorUtility.Calls, Has.Count.EqualTo(1));
            Assert.That(AssetDatabase.RefreshCallCount, Is.EqualTo(0));
            Assert.That(Debug.Entries, Is.Empty);
        }

        /// <summary>
        /// Tests that create default config logs errors when write fails.
        /// </summary>
        [Test]
        public void CreateDefaultConfig_LogsErrorsWhenWriteFails()
        {
            using var scope = new TestProjectScope();
            var configDirectory = scope.GetProjectPath(ConfigLoader.FileName);
            Directory.CreateDirectory(configDirectory);

            InvokeCreateDefaultConfig();

            Assert.That(Debug.Entries.Last().Level, Is.EqualTo("Error"));
            Assert.That(Debug.Entries.Last().Message, Does.Contain("Failed to create TailwindUSS config:"));
        }

        /// <summary>
        /// Tests that generate and validate menu commands invoke services.
        /// </summary>
        [Test]
        public void GenerateAndValidateMenuCommandsInvokeServices()
        {
            using var scope = new TestProjectScope();
            scope.WriteAssetFile("UI/Main.uxml", "<ui:UXML xmlns:ui=\"UnityEngine.UIElements\"><ui:VisualElement class=\"flex\" /></ui:UXML>");

            InvokeNonPublicStatic("Generate");
            InvokeNonPublicStatic("Validate");

            Assert.That(Debug.Entries[0].Message, Does.Contain("config was not found"));
            Assert.That(Debug.Entries.Any(entry => entry.Message.Contains("TailwindUSS generation finished.")), Is.True);
            Assert.That(Debug.Entries.Any(entry => entry.Message.Contains("TailwindUSS validation finished.")), Is.True);
        }

        /// <summary>
        /// Tests that settings menu command opens project settings.
        /// </summary>
        [Test]
        public void SettingsMenuCommand_OpensProjectSettings()
        {
            using var scope = new TestProjectScope();

            InvokeNonPublicStatic("OpenSettings");

            Assert.That(SettingsService.LastOpenedProjectSettingsPath, Is.EqualTo(TailwindUssSettingsProvider.SettingsPath));
        }

        private static void InvokeCreateDefaultConfig()
        {
            InvokeNonPublicStatic("CreateDefaultConfig");
        }

        private static void InvokeNonPublicStatic(string methodName)
        {
            var method = typeof(TailwindUssMenu).GetMethod(methodName, BindingFlags.Static | BindingFlags.NonPublic);
            Assert.That(method, Is.Not.Null);
            method.Invoke(null, null);
        }
    }
}
