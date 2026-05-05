using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace TailwindUSS.Editor
{
    internal sealed class UxmlStyleReferenceUpdater
    {
        public void EnsureStyleReference(string projectRoot, IEnumerable<string> relativeFilePaths, string outputUssPath, IList<TailwindUssDiagnostic> diagnostics)
        {
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

                    if (HasMatchingStyleReference(document.Root, outputUssPath))
                    {
                        continue;
                    }

                    var styleElement = new XElement(document.Root.Name.Namespace + "Style", new XAttribute("src", outputUssPath));
                    document.Root.AddFirst(styleElement);
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

        private static bool HasMatchingStyleReference(XElement root, string outputUssPath)
        {
            foreach (var child in root.Elements())
            {
                if (!string.Equals(child.Name.LocalName, "Style", StringComparison.Ordinal))
                {
                    continue;
                }

                var srcAttribute = child.Attribute("src");
                if (srcAttribute != null && string.Equals(srcAttribute.Value, outputUssPath, StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
