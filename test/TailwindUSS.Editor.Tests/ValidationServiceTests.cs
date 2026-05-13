using System.Linq;

namespace TailwindUSS.Editor.Tests
{
    /// <summary>
    /// Represents the validation service tests.
    /// </summary>
    public sealed class ValidationServiceTests
    {
        /// <summary>
        /// Tests that validate load failure returns error.
        /// </summary>
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

        /// <summary>
        /// Tests that validate counts supported warnings and errors.
        /// </summary>
        [Test]
        public void Validate_CountsSupportedWarningsAndErrors()
        {
            using var scope = new TestProjectScope();
            scope.WriteAssetFile("UI/Main.uxml", "<ui:UXML xmlns:ui=\"UnityEngine.UIElements\"><ui:VisualElement class=\"flex p-7 unknown uppercase unknown\" /></ui:UXML>");

            var result = new ValidationService().Validate();

            Assert.That(result.GeneratedUtilityCount, Is.EqualTo(1));
            Assert.That(result.WarningCount, Is.EqualTo(4));
            Assert.That(result.ErrorCount, Is.EqualTo(0));
            Assert.That(Debug.Entries.Any(entry => entry.Message.Contains("Duplicate token 'unknown'")), Is.True);
            Assert.That(Debug.Entries.Any(entry => entry.Message.Contains("Invalid utility token 'p-7'")), Is.True);
            Assert.That(Debug.Entries.Any(entry => entry.Message.Contains("Unsupported utility token 'unknown'")), Is.True);
            Assert.That(Debug.Entries.Any(entry => entry.Message.Contains("Unsupported utility token 'uppercase': Unity USS does not support text-transform; reproducing it requires changing the source text in text/value, not styling.")), Is.True);
            Assert.That(Debug.Entries.Last().Message, Does.Contain("TailwindUSS validation finished."));
        }

        /// <summary>
        /// Tests that validate reports gap utilities as unsupported.
        /// </summary>
        [Test]
        public void Validate_ReportsGapUtilitiesAsUnsupported()
        {
            using var scope = new TestProjectScope();
            scope.WriteAssetFile("UI/Main.uxml", "<ui:UXML xmlns:ui=\"UnityEngine.UIElements\"><ui:VisualElement class=\"gap-4 gap-x-2 gap-y-3\" /></ui:UXML>");

            var result = new ValidationService().Validate();

            Assert.That(result.GeneratedUtilityCount, Is.EqualTo(0));
            Assert.That(result.WarningCount, Is.EqualTo(3));
            Assert.That(result.ErrorCount, Is.EqualTo(0));
            Assert.That(Debug.Entries.Any(entry => entry.Message.Contains("Unsupported utility token 'gap-4': Unity USS does not support gap, row-gap, or column-gap; exact reproduction would require structural selectors that UI Toolkit USS lacks.")), Is.True);
            Assert.That(Debug.Entries.Any(entry => entry.Message.Contains("Unsupported utility token 'gap-x-2': Unity USS does not support gap, row-gap, or column-gap; exact reproduction would require structural selectors that UI Toolkit USS lacks.")), Is.True);
            Assert.That(Debug.Entries.Any(entry => entry.Message.Contains("Unsupported utility token 'gap-y-3': Unity USS does not support gap, row-gap, or column-gap; exact reproduction would require structural selectors that UI Toolkit USS lacks.")), Is.True);
        }

        /// <summary>
        /// Tests that validate reports leading utilities as unsupported.
        /// </summary>
        [Test]
        public void Validate_ReportsLeadingUtilitiesAsUnsupported()
        {
            using var scope = new TestProjectScope();
            scope.WriteAssetFile("UI/Main.uxml", "<ui:UXML xmlns:ui=\"UnityEngine.UIElements\"><ui:Label class=\"leading-5 leading-6\" /></ui:UXML>");

            var result = new ValidationService().Validate();

            Assert.That(result.GeneratedUtilityCount, Is.EqualTo(0));
            Assert.That(result.WarningCount, Is.EqualTo(2));
            Assert.That(result.ErrorCount, Is.EqualTo(0));
            Assert.That(Debug.Entries.Any(entry => entry.Message.Contains("Unsupported utility token 'leading-5': Unity USS does not support line-height; paragraph spacing only affects paragraph breaks and cannot reproduce leading utilities.")), Is.True);
            Assert.That(Debug.Entries.Any(entry => entry.Message.Contains("Unsupported utility token 'leading-6': Unity USS does not support line-height; paragraph spacing only affects paragraph breaks and cannot reproduce leading utilities.")), Is.True);
        }

        /// <summary>
        /// Tests that validate reports duplicate and invalid filter utilities.
        /// </summary>
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
