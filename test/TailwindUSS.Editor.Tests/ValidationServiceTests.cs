using System.Linq;

namespace TailwindUSS.Editor.Tests
{
    public sealed class ValidationServiceTests
    {
        [Test]
        public void Validate_LoadFailureReturnsError()
        {
            using var scope = new TestProjectScope();
            scope.WriteProjectFile(ConfigLoader.FileName, "{ invalid json }");

            var result = new ValidationService().Validate();

            Assert.That(result.ErrorCount, Is.EqualTo(1));
            Assert.That(result.GeneratedUtilityCount, Is.EqualTo(0));
            Assert.That(Debug.Entries.Last().Level, Is.EqualTo("Error"));
        }

        [Test]
        public void Validate_CountsSupportedWarningsAndErrors()
        {
            using var scope = new TestProjectScope();
            scope.WriteAssetFile("UI/Main.uxml", "<ui:UXML xmlns:ui=\"UnityEngine.UIElements\"><ui:VisualElement class=\"flex p-7 unknown unknown\" /></ui:UXML>");

            var result = new ValidationService().Validate();

            Assert.That(result.GeneratedUtilityCount, Is.EqualTo(1));
            Assert.That(result.WarningCount, Is.EqualTo(3));
            Assert.That(result.ErrorCount, Is.EqualTo(0));
            Assert.That(Debug.Entries.Any(entry => entry.Message.Contains("Duplicate token 'unknown'")), Is.True);
            Assert.That(Debug.Entries.Any(entry => entry.Message.Contains("Invalid utility token 'p-7'")), Is.True);
            Assert.That(Debug.Entries.Any(entry => entry.Message.Contains("Unsupported utility token 'unknown'")), Is.True);
            Assert.That(Debug.Entries.Last().Message, Does.Contain("TailwindUSS validation finished."));
        }

        [Test]
        public void Validate_ReportsDuplicateAndInvalidFilterUtilities()
        {
            using var scope = new TestProjectScope();
            scope.WriteAssetFile("UI/Main.uxml", """
            <ui:UXML xmlns:ui="UnityEngine.UIElements">
                <ui:VisualElement class="blur-sm blur-lg grayscale contrast-110" />
            </ui:UXML>
            """);

            var result = new ValidationService().Validate();

            Assert.That(result.GeneratedUtilityCount, Is.EqualTo(3));
            Assert.That(result.WarningCount, Is.EqualTo(2));
            Assert.That(result.ErrorCount, Is.EqualTo(0));
            Assert.That(Debug.Entries.Any(entry => entry.Message.Contains("Duplicate filter family 'blur'")), Is.True);
            Assert.That(Debug.Entries.Any(entry => entry.Message.Contains("Invalid utility token 'contrast-110'")), Is.True);
        }
    }
}
