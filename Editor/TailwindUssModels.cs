using System.Collections.Generic;

namespace TailwindUSS.Editor
{
    internal enum DiagnosticSeverity
    {
        Warning,
        Error
    }

    internal enum TokenIssueKind
    {
        Unsupported,
        InvalidValue,
        Duplicate
    }

    internal enum ResolveStatus
    {
        Supported,
        Unsupported,
        InvalidValue
    }

    internal sealed class TailwindUssDiagnostic
    {
        public TailwindUssDiagnostic(
            DiagnosticSeverity severity,
            TokenIssueKind? issueKind,
            string message,
            string relativeFilePath,
            int lineNumber,
            string elementName,
            string token)
        {
            Severity = severity;
            IssueKind = issueKind;
            Message = message;
            RelativeFilePath = relativeFilePath;
            LineNumber = lineNumber;
            ElementName = elementName;
            Token = token;
        }

        public DiagnosticSeverity Severity { get; private set; }
        public TokenIssueKind? IssueKind { get; private set; }
        public string Message { get; private set; }
        public string RelativeFilePath { get; private set; }
        public int LineNumber { get; private set; }
        public string ElementName { get; private set; }
        public string Token { get; private set; }
    }

    internal sealed class UxmlTokenOccurrence
    {
        public UxmlTokenOccurrence(string relativeFilePath, int lineNumber, string elementName, string token)
        {
            RelativeFilePath = relativeFilePath;
            LineNumber = lineNumber;
            ElementName = elementName;
            Token = token;
        }

        public string RelativeFilePath { get; private set; }
        public int LineNumber { get; private set; }
        public string ElementName { get; private set; }
        public string Token { get; private set; }
    }

    internal sealed class StyleDeclaration
    {
        public StyleDeclaration(string propertyName, string value)
        {
            PropertyName = propertyName;
            Value = value;
        }

        public string PropertyName { get; private set; }
        public string Value { get; private set; }
    }

    internal sealed class ResolvedUtility
    {
        public ResolvedUtility(string token, IList<StyleDeclaration> declarations)
        {
            Token = token;
            Declarations = declarations;
        }

        public string Token { get; private set; }
        public IList<StyleDeclaration> Declarations { get; private set; }
    }

    internal sealed class UxmlScanResult
    {
        public UxmlScanResult()
        {
            MatchedFiles = new List<string>();
            Occurrences = new List<UxmlTokenOccurrence>();
            Diagnostics = new List<TailwindUssDiagnostic>();
        }

        public IList<string> MatchedFiles { get; private set; }
        public IList<UxmlTokenOccurrence> Occurrences { get; private set; }
        public IList<TailwindUssDiagnostic> Diagnostics { get; private set; }
    }

    internal sealed class CommandResult
    {
        public string OutputAssetPath;
        public int GeneratedUtilityCount;
        public int WarningCount;
        public int ErrorCount;
    }
}
