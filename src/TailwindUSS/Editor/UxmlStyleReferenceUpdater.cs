using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace TailwindUSS.Editor
{
    /// <summary>
    /// Represents the uxml style reference updater.
    /// </summary>
    internal sealed class UxmlStyleReferenceUpdater
    {
        private const string ProjectAssetUriPrefix = "project://database/";

        /// <summary>
        /// Ensures style reference.
        /// </summary>
        public void EnsureStyleReference(string projectRoot, IEnumerable<string> relativeFilePaths, string outputUssPath, IList<TailwindUssDiagnostic> diagnostics)
        {
            var canonicalStyleReference = CreateCanonicalStyleReference(outputUssPath);

            foreach (var relativeFilePath in relativeFilePaths)
            {
                var absolutePath = Path.Combine(projectRoot, relativeFilePath.Replace('/', Path.DirectorySeparatorChar));

                try
                {
                    var document = XDocument.Load(absolutePath, LoadOptions.PreserveWhitespace);
                    if (document.Root == null)
                    {
                        continue;
                    }

                    if (!EnsureCanonicalStyleReference(projectRoot, relativeFilePath, document.Root, outputUssPath, canonicalStyleReference))
                    {
                        continue;
                    }

                    document.Save(absolutePath, SaveOptions.DisableFormatting);
                }
                catch (Exception exception)
                {
                    diagnostics.Add(new TailwindUssDiagnostic(
                        DiagnosticSeverity.Error,
                        null,
                        string.Format("Failed to update UXML style reference: {0}", exception.Message),
                        relativeFilePath,
                        0,
                        string.Empty,
                        string.Empty));
                }
            }
        }

        private static bool EnsureCanonicalStyleReference(string projectRoot, string relativeFilePath, XElement root, string outputUssPath, string canonicalStyleReference)
        {
            var hasMatchingReference = false;
            var changed = false;

            foreach (var child in new List<XElement>(root.Elements()))
            {
                if (!string.Equals(child.Name.LocalName, "Style", StringComparison.Ordinal))
                {
                    continue;
                }

                var srcAttribute = child.Attribute("src");
                if (srcAttribute == null)
                {
                    continue;
                }

                if (!TryResolveAssetPath(projectRoot, relativeFilePath, srcAttribute.Value, out var referencedAssetPath)
                    || !string.Equals(referencedAssetPath, outputUssPath, StringComparison.Ordinal))
                {
                    continue;
                }

                if (!hasMatchingReference)
                {
                    if (!string.Equals(srcAttribute.Value, canonicalStyleReference, StringComparison.Ordinal))
                    {
                        srcAttribute.Value = canonicalStyleReference;
                        changed = true;
                    }

                    hasMatchingReference = true;
                    continue;
                }

                child.Remove();
                changed = true;
            }

            if (hasMatchingReference)
            {
                return changed;
            }

            var styleElement = new XElement(root.Name.Namespace + "Style", new XAttribute("src", canonicalStyleReference));
            root.AddFirst(styleElement);
            return true;
        }

        private static string CreateCanonicalStyleReference(string outputUssPath)
        {
            return outputUssPath.StartsWith(ProjectAssetUriPrefix, StringComparison.Ordinal)
                ? StripQueryAndFragment(outputUssPath)
                : ProjectAssetUriPrefix + outputUssPath;
        }

        private static bool TryResolveAssetPath(string projectRoot, string relativeFilePath, string src, out string assetPath)
        {
            assetPath = null;
            if (string.IsNullOrWhiteSpace(src))
            {
                return false;
            }

            if (src.StartsWith(ProjectAssetUriPrefix, StringComparison.Ordinal))
            {
                assetPath = StripQueryAndFragment(src.Substring(ProjectAssetUriPrefix.Length));
                return true;
            }

            if (src.StartsWith("Assets/", StringComparison.Ordinal))
            {
                assetPath = src.Replace('\\', '/');
                return true;
            }

            var assetDirectory = Path.GetDirectoryName(relativeFilePath.Replace('/', Path.DirectorySeparatorChar)) ?? string.Empty;
            var referencedAbsolutePath = Path.GetFullPath(Path.Combine(projectRoot, assetDirectory, src.Replace('/', Path.DirectorySeparatorChar)));
            if (!referencedAbsolutePath.StartsWith(projectRoot, StringComparison.Ordinal))
            {
                return false;
            }

            assetPath = NormalizeAssetPath(projectRoot, referencedAbsolutePath);
            return assetPath.StartsWith("Assets/", StringComparison.Ordinal);
        }

        private static string NormalizeAssetPath(string projectRoot, string absolutePath)
        {
            var relativePath = absolutePath.Substring(projectRoot.Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            return relativePath.Replace('\\', '/');
        }

        private static string StripQueryAndFragment(string value)
        {
            var separatorIndex = value.IndexOfAny(new[] { '?', '#' });
            return separatorIndex >= 0 ? value.Substring(0, separatorIndex) : value;
        }
    }
}
