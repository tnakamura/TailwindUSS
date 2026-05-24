using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace TailwindUSS.Editor
{
    /// <summary>
    /// Represents the uxml scanner.
    /// </summary>
    internal sealed class UxmlScanner
    {
        private readonly ClassTokenParser classTokenParser = new ClassTokenParser();

        /// <summary>
        /// Scans UXML files matching the input globs for utility class tokens.
        /// </summary>
        public UxmlScanResult Scan(string projectRoot, IEnumerable<string> inputGlobs)
        {
            var result = new UxmlScanResult();
            var regexes = BuildRegexes(inputGlobs);
            var nextClassAttributeId = 1;

            foreach (var filePath in Directory.GetFiles(projectRoot, "*.uxml", SearchOption.AllDirectories))
            {
                var relativeFilePath = MakeRelativeProjectPath(projectRoot, filePath);
                if (!MatchesAny(relativeFilePath, regexes))
                {
                    continue;
                }

                result.MatchedFiles.Add(relativeFilePath);
                nextClassAttributeId = AppendFileScanResult(result, ScanMatchedFile(projectRoot, relativeFilePath), nextClassAttributeId);
            }

            return result;
        }

        /// <summary>
        /// Determines whether the relative path matches the configured input globs.
        /// </summary>
        public bool MatchesInputGlobs(string relativeFilePath, IEnumerable<string> inputGlobs)
        {
            return MatchesAny(NormalizePath(relativeFilePath), BuildRegexes(inputGlobs));
        }

        /// <summary>
        /// Scans a single matched UXML file.
        /// </summary>
        public UxmlFileScanResult ScanMatchedFile(string projectRoot, string relativeFilePath)
        {
            var result = new UxmlFileScanResult(NormalizePath(relativeFilePath));
            var nextClassAttributeId = 1;
            var absolutePath = Path.Combine(projectRoot, relativeFilePath.Replace('/', Path.DirectorySeparatorChar));

            try
            {
                var document = XDocument.Load(absolutePath, LoadOptions.SetLineInfo);
                if (document.Root == null)
                {
                    return result;
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
                        result.RelativeFilePath,
                        lineNumber,
                        element.Name.LocalName,
                        result.Diagnostics,
                        nextClassAttributeId++);

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
                    result.RelativeFilePath,
                    0,
                    string.Empty,
                    string.Empty));
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

        private static int AppendFileScanResult(UxmlScanResult scanResult, UxmlFileScanResult fileResult, int nextClassAttributeId)
        {
            foreach (var diagnostic in fileResult.Diagnostics)
            {
                scanResult.Diagnostics.Add(diagnostic);
            }

            return AppendOccurrencesWithRemappedClassAttributeIds(scanResult.Occurrences, fileResult.Occurrences, nextClassAttributeId);
        }

        internal static int AppendOccurrencesWithRemappedClassAttributeIds(
            ICollection<UxmlTokenOccurrence> target,
            IEnumerable<UxmlTokenOccurrence> occurrences,
            int nextClassAttributeId)
        {
            var classAttributeIdMap = new Dictionary<int, int>();

            foreach (var occurrence in occurrences)
            {
                if (!classAttributeIdMap.TryGetValue(occurrence.ClassAttributeId, out var remappedClassAttributeId))
                {
                    remappedClassAttributeId = nextClassAttributeId++;
                    classAttributeIdMap.Add(occurrence.ClassAttributeId, remappedClassAttributeId);
                }

                target.Add(new UxmlTokenOccurrence(
                    occurrence.RelativeFilePath,
                    occurrence.LineNumber,
                    occurrence.ElementName,
                    occurrence.OriginalToken,
                    occurrence.VariantChain,
                    occurrence.BaseToken,
                    remappedClassAttributeId));
            }

            return nextClassAttributeId;
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

            /// <summary>
            /// Appends a string value to the builder.
            /// </summary>
            public void Append(string value)
            {
                stringBuilder.Append(value);
            }

            /// <summary>
            /// Returns the accumulated string value.
            /// </summary>
            public override string ToString()
            {
                return stringBuilder.ToString();
            }
        }
    }
}
