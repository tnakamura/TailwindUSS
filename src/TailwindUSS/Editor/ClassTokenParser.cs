using System;
using System.Collections.Generic;

namespace TailwindUSS.Editor
{
    /// <summary>
    /// Represents the class token parser.
    /// </summary>
    internal sealed class ClassTokenParser
    {
        /// <summary>
        /// Parses the operation.
        /// </summary>
        public IList<UxmlTokenOccurrence> Parse(string classValue, string relativeFilePath, int lineNumber, string elementName, IList<TailwindUssDiagnostic> diagnostics, int classAttributeId = 0)
        {
            var tokens = new List<UxmlTokenOccurrence>();
            var seen = new HashSet<string>(StringComparer.Ordinal);
            var parts = classValue.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);

            foreach (var part in parts)
            {
                if (!seen.Add(part))
                {
                    diagnostics.Add(new TailwindUssDiagnostic(
                        DiagnosticSeverity.Warning,
                        TokenIssueKind.Duplicate,
                        string.Format("Duplicate token '{0}' in class attribute.", part),
                        relativeFilePath,
                        lineNumber,
                        elementName,
                        part));
                    continue;
                }

                var segments = part.Split(':');
                var variantChain = new List<string>();
                for (var i = 0; i < segments.Length - 1; i++)
                {
                    variantChain.Add(segments[i]);
                }

                tokens.Add(new UxmlTokenOccurrence(
                    relativeFilePath,
                    lineNumber,
                    elementName,
                    part,
                    variantChain,
                    segments[segments.Length - 1],
                    classAttributeId));
            }

            return tokens;
        }
    }
}
