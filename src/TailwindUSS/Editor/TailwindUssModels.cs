using System.Collections.Generic;

namespace TailwindUSS.Editor
{
    /// <summary>
    /// Defines the diagnostic severity.
    /// </summary>
    internal enum DiagnosticSeverity
    {
        Warning,
        Error
    }

    /// <summary>
    /// Defines the token issue kind.
    /// </summary>
    internal enum TokenIssueKind
    {
        Unsupported,
        UnsupportedVariant,
        InvalidValue,
        Duplicate
    }

    /// <summary>
    /// Defines the resolve status.
    /// </summary>
    internal enum ResolveStatus
    {
        Supported,
        Unsupported,
        UnsupportedVariant,
        InvalidValue
    }

    /// <summary>
    /// Represents the tailwind uss diagnostic.
    /// </summary>
    internal sealed class TailwindUssDiagnostic
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TailwindUssDiagnostic"/> type.
        /// </summary>
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

        /// <summary>
        /// Gets the severity.
        /// </summary>
        public DiagnosticSeverity Severity { get; private set; }
        /// <summary>
        /// Gets the issue kind.
        /// </summary>
        public TokenIssueKind? IssueKind { get; private set; }
        /// <summary>
        /// Gets the message.
        /// </summary>
        public string Message { get; private set; }
        /// <summary>
        /// Gets the relative file path.
        /// </summary>
        public string RelativeFilePath { get; private set; }
        /// <summary>
        /// Gets the line number.
        /// </summary>
        public int LineNumber { get; private set; }
        /// <summary>
        /// Gets the element name.
        /// </summary>
        public string ElementName { get; private set; }
        /// <summary>
        /// Gets the token.
        /// </summary>
        public string Token { get; private set; }
    }

    /// <summary>
    /// Represents the uxml token occurrence.
    /// </summary>
    internal sealed class UxmlTokenOccurrence
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UxmlTokenOccurrence"/> type.
        /// </summary>
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

        /// <summary>
        /// Gets the relative file path.
        /// </summary>
        public string RelativeFilePath { get; private set; }
        /// <summary>
        /// Gets the line number.
        /// </summary>
        public int LineNumber { get; private set; }
        /// <summary>
        /// Gets the element name.
        /// </summary>
        public string ElementName { get; private set; }
        /// <summary>
        /// Gets the original token.
        /// </summary>
        public string OriginalToken { get; private set; }
        /// <summary>
        /// Gets the variant chain.
        /// </summary>
        public IList<string> VariantChain { get; private set; }
        /// <summary>
        /// Gets the base token.
        /// </summary>
        public string BaseToken { get; private set; }
        /// <summary>
        /// Gets the class attribute identifier.
        /// </summary>
        public int ClassAttributeId { get; private set; }
    }

    /// <summary>
    /// Represents the style declaration.
    /// </summary>
    internal sealed class StyleDeclaration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StyleDeclaration"/> type.
        /// </summary>
        public StyleDeclaration(string propertyName, string value)
        {
            PropertyName = propertyName;
            Value = value;
        }

        /// <summary>
        /// Gets the property name.
        /// </summary>
        public string PropertyName { get; private set; }
        /// <summary>
        /// Gets the value.
        /// </summary>
        public string Value { get; private set; }
    }

    /// <summary>
    /// Represents the resolved utility.
    /// </summary>
    internal sealed class ResolvedUtility
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResolvedUtility"/> type.
        /// </summary>
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

        /// <summary>
        /// Gets the token.
        /// </summary>
        public string Token { get; private set; }
        /// <summary>
        /// Gets the declarations.
        /// </summary>
        public IList<StyleDeclaration> Declarations { get; private set; }
        /// <summary>
        /// Gets the selector suffix.
        /// </summary>
        public string SelectorSuffix { get; private set; }
        /// <summary>
        /// Gets the selector override.
        /// </summary>
        public string SelectorOverride { get; private set; }
        /// <summary>
        /// Gets the filter contribution.
        /// </summary>
        public FilterContribution FilterContribution { get; private set; }
        /// <summary>
        /// Gets the is filter utility.
        /// </summary>
        public bool IsFilterUtility => FilterContribution != null;
    }

    /// <summary>
    /// Represents the filter contribution.
    /// </summary>
    internal sealed class FilterContribution
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FilterContribution"/> type.
        /// </summary>
        public FilterContribution(string family, string function)
        {
            Family = family;
            Function = function;
        }

        /// <summary>
        /// Gets the family.
        /// </summary>
        public string Family { get; private set; }
        /// <summary>
        /// Gets the function.
        /// </summary>
        public string Function { get; private set; }
    }

    /// <summary>
    /// Represents the resolved token occurrence.
    /// </summary>
    internal sealed class ResolvedTokenOccurrence
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResolvedTokenOccurrence"/> type.
        /// </summary>
        public ResolvedTokenOccurrence(UxmlTokenOccurrence occurrence, ResolvedUtility utility)
        {
            Occurrence = occurrence;
            Utility = utility;
        }

        /// <summary>
        /// Gets the occurrence.
        /// </summary>
        public UxmlTokenOccurrence Occurrence { get; private set; }
        /// <summary>
        /// Gets the utility.
        /// </summary>
        public ResolvedUtility Utility { get; private set; }
    }

    /// <summary>
    /// Represents the uxml scan result.
    /// </summary>
    internal sealed class UxmlScanResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UxmlScanResult"/> type.
        /// </summary>
        public UxmlScanResult()
        {
            MatchedFiles = new List<string>();
            Occurrences = new List<UxmlTokenOccurrence>();
            Diagnostics = new List<TailwindUssDiagnostic>();
        }

        /// <summary>
        /// Gets the matched files.
        /// </summary>
        public IList<string> MatchedFiles { get; private set; }
        /// <summary>
        /// Gets the occurrences.
        /// </summary>
        public IList<UxmlTokenOccurrence> Occurrences { get; private set; }
        /// <summary>
        /// Gets the diagnostics.
        /// </summary>
        public IList<TailwindUssDiagnostic> Diagnostics { get; private set; }
    }

    /// <summary>
    /// Represents the command result.
    /// </summary>
    internal sealed class CommandResult
    {
        /// <summary>
        /// Gets or sets the output asset path.
        /// </summary>
        public string OutputAssetPath;
        /// <summary>
        /// Gets or sets the generated utility count.
        /// </summary>
        public int GeneratedUtilityCount;
        /// <summary>
        /// Gets or sets the warning count.
        /// </summary>
        public int WarningCount;
        /// <summary>
        /// Gets or sets the error count.
        /// </summary>
        public int ErrorCount;
    }
}
