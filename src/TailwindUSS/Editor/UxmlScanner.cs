using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace TailwindUSS.Editor
{
    internal sealed class UxmlScanner
    {
        private readonly ClassTokenParser classTokenParser = new ClassTokenParser();

        public UxmlScanResult Scan(string projectRoot, IEnumerable<string> inputGlobs)
        {
            var result = new UxmlScanResult();
            var regexes = BuildRegexes(inputGlobs);
            var classAttributeId = 1;

            foreach (var filePath in Directory.GetFiles(projectRoot, "*.uxml", SearchOption.AllDirectories))
            {
                var relativeFilePath = MakeRelativeProjectPath(projectRoot, filePath);
                if (!MatchesAny(relativeFilePath, regexes))
                {
                    continue;
                }

                result.MatchedFiles.Add(relativeFilePath);

                try
                {
                    var document = XDocument.Load(filePath, LoadOptions.SetLineInfo);
                    if (document.Root == null)
                    {
                        continue;
                    }

                    foreach (var element in document.Root.DescendantsAndSelf())
                    {
                        var classAttribute = element.Attribute("class");
                        if (classAttribute == null || string.IsNullOrWhiteSpace(classAttribute.Value))
                        {
                            continue;
                        }

                        var lineInfo = (IXmlLineInfo)element;
                        var lineNumber = lineInfo.HasLineInfo() ? lineInfo.LineNumber : 0;
                        var tokens = classTokenParser.Parse(
                            classAttribute.Value,
                            relativeFilePath,
                            lineNumber,
                            element.Name.LocalName,
                            result.Diagnostics,
                            classAttributeId++);

                        foreach (var token in tokens)
                        {
                            result.Occurrences.Add(token);
                        }
                    }
                }
                catch (Exception exception)
                {
                    result.Diagnostics.Add(new TailwindUssDiagnostic(
                        DiagnosticSeverity.Error,
                        null,
                        string.Format("Failed to parse UXML: {0}", exception.Message),
                        relativeFilePath,
                        0,
                        string.Empty,
                        string.Empty));
                }
            }

            return result;
        }

        private static bool MatchesAny(string relativeFilePath, IList<Regex> regexes)
        {
            foreach (var regex in regexes)
            {
                if (regex.IsMatch(relativeFilePath))
                {
                    return true;
                }
            }

            return false;
        }

        private static IList<Regex> BuildRegexes(IEnumerable<string> inputGlobs)
        {
            var regexes = new List<Regex>();

            foreach (var inputGlob in inputGlobs)
            {
                if (string.IsNullOrWhiteSpace(inputGlob))
                {
                    continue;
                }

                regexes.Add(new Regex(ConvertGlobToRegex(NormalizePath(inputGlob)), RegexOptions.Compiled | RegexOptions.CultureInvariant));
            }

            if (regexes.Count == 0)
            {
                regexes.Add(new Regex("^Assets/.*\\.uxml$", RegexOptions.Compiled | RegexOptions.CultureInvariant));
            }

            return regexes;
        }

        private static string MakeRelativeProjectPath(string projectRoot, string filePath)
        {
            var relativePath = filePath.Substring(projectRoot.Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            return NormalizePath(relativePath);
        }

        private static string NormalizePath(string path)
        {
            return path.Replace('\\', '/');
        }

        private static string ConvertGlobToRegex(string glob)
        {
            var pattern = new RegexBuilder();
            pattern.Append("^");

            for (var index = 0; index < glob.Length; index++)
            {
                var character = glob[index];
                if (character == '*')
                {
                    var isGlobStar = index + 1 < glob.Length && glob[index + 1] == '*';
                    if (isGlobStar)
                    {
                        var consumesSlash = index + 2 < glob.Length && glob[index + 2] == '/';
                        pattern.Append(consumesSlash ? "(?:.*/)?" : ".*");
                        index += consumesSlash ? 2 : 1;
                        continue;
                    }

                    pattern.Append("[^/]*");
                    continue;
                }

                if (character == '?')
                {
                    pattern.Append("[^/]");
                    continue;
                }

                if (character == '/')
                {
                    pattern.Append("/");
                    continue;
                }

                pattern.Append(Regex.Escape(character.ToString()));
            }

            pattern.Append("$");
            return pattern.ToString();
        }

        private sealed class RegexBuilder
        {
            private readonly System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();

            public void Append(string value)
            {
                stringBuilder.Append(value);
            }

            public override string ToString()
            {
                return stringBuilder.ToString();
            }
        }
    }
}
