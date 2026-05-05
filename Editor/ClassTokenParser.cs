using System;
using System.Collections.Generic;

namespace TailwindUSS.Editor
{
    internal sealed class ClassTokenParser
    {
        public IList<string> Parse(string classValue, string relativeFilePath, int lineNumber, string elementName, IList<TailwindUssDiagnostic> diagnostics)
        {
            var tokens = new List<string>();
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

                tokens.Add(part);
            }

            return tokens;
        }
    }
}
