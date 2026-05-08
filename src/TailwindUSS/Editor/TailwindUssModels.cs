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
        UnsupportedVariant,
        InvalidValue,
        Duplicate
    }

    internal enum ResolveStatus
    {
        Supported,
        Unsupported,
        UnsupportedVariant,
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
        public UxmlTokenOccurrence(
            string relativeFilePath,
            int lineNumber,
            string elementName,
            string originalToken,
            IList<string> variantChain,
            string baseToken,
            int classAttributeId = 0)
        {
            RelativeFilePath = relativeFilePath;
            LineNumber = lineNumber;
            ElementName = elementName;
            OriginalToken = originalToken;
            VariantChain = variantChain;
            BaseToken = baseToken;
            ClassAttributeId = classAttributeId;
        }

        public string RelativeFilePath { get; private set; }
        public int LineNumber { get; private set; }
        public string ElementName { get; private set; }
        public string OriginalToken { get; private set; }
        public IList<string> VariantChain { get; private set; }
        public string BaseToken { get; private set; }
        public int ClassAttributeId { get; private set; }
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
        public ResolvedUtility(
            string token,
            IList<StyleDeclaration> declarations,
            string selectorSuffix = "",
            string selectorOverride = null,
            FilterContribution filterContribution = null)
        {
            Token = token;
            Declarations = declarations;
            SelectorSuffix = selectorSuffix;
            SelectorOverride = selectorOverride;
            FilterContribution = filterContribution;
        }

        public string Token { get; private set; }
        public IList<StyleDeclaration> Declarations { get; private set; }
        public string SelectorSuffix { get; private set; }
        public string SelectorOverride { get; private set; }
        public FilterContribution FilterContribution { get; private set; }
        public bool IsFilterUtility => FilterContribution != null;
    }

    internal sealed class FilterContribution
    {
        public FilterContribution(string family, string function)
        {
            Family = family;
            Function = function;
        }

        public string Family { get; private set; }
        public string Function { get; private set; }
    }

    internal sealed class ResolvedTokenOccurrence
    {
        public ResolvedTokenOccurrence(UxmlTokenOccurrence occurrence, ResolvedUtility utility)
        {
            Occurrence = occurrence;
            Utility = utility;
        }

        public UxmlTokenOccurrence Occurrence { get; private set; }
        public ResolvedUtility Utility { get; private set; }
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
