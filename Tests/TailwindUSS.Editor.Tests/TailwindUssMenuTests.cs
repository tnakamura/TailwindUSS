using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace TailwindUSS.Editor.Tests
{
    public sealed class TailwindUssMenuTests
    {
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
