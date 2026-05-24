using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace TailwindUSS.Editor
{
    /// <summary>
    /// Caches per-file UXML scan results to support incremental USS generation.
    /// </summary>
    internal sealed class IncrementalGenerationCache
    {
        private readonly UxmlScanner scanner = new UxmlScanner();
        private readonly Dictionary<string, UxmlFileScanResult> fileScanResults = new Dictionary<string, UxmlFileScanResult>(StringComparer.Ordinal);
        private string projectRoot;
        private string configSignature;

        /// <summary>
        /// Gets the shared cache instance.
        /// </summary>
        internal static IncrementalGenerationCache Shared { get; } = new IncrementalGenerationCache();

        /// <summary>
        /// Clears the cached state.
        /// </summary>
        internal void Reset()
        {
            projectRoot = null;
            configSignature = null;
            fileScanResults.Clear();
        }

        /// <summary>
        /// Updates the cache from a full scan result.
        /// </summary>
        internal void UpdateFromFullScan(string currentProjectRoot, TailwindUssConfig config, UxmlScanResult scanResult)
        {
            projectRoot = currentProjectRoot;
            configSignature = CreateConfigSignature(config);
            fileScanResults.Clear();

            foreach (var matchedFile in scanResult.MatchedFiles)
            {
                fileScanResults[matchedFile] = new UxmlFileScanResult(matchedFile);
            }

            foreach (var diagnostic in scanResult.Diagnostics)
            {
                if (string.IsNullOrEmpty(diagnostic.RelativeFilePath))
                {
                    continue;
                }

                if (!fileScanResults.TryGetValue(diagnostic.RelativeFilePath, out var fileResult))
                {
                    fileResult = new UxmlFileScanResult(diagnostic.RelativeFilePath);
                    fileScanResults.Add(diagnostic.RelativeFilePath, fileResult);
                }

                fileResult.Diagnostics.Add(diagnostic);
            }

            foreach (var occurrence in scanResult.Occurrences)
            {
                if (!fileScanResults.TryGetValue(occurrence.RelativeFilePath, out var fileResult))
                {
                    fileResult = new UxmlFileScanResult(occurrence.RelativeFilePath);
                    fileScanResults.Add(occurrence.RelativeFilePath, fileResult);
                }

                fileResult.Occurrences.Add(occurrence);
            }
        }

        /// <summary>
        /// Applies changed and deleted UXML assets and returns the aggregated scan result.
        /// </summary>
        internal UxmlScanResult UpdateAndBuildScan(string currentProjectRoot, TailwindUssConfig config, IEnumerable<string> changedAssetPaths, IEnumerable<string> deletedAssetPaths)
        {
            EnsureInitialized(currentProjectRoot, config);

            foreach (var assetPath in NormalizeAssetPaths(deletedAssetPaths))
            {
                fileScanResults.Remove(assetPath);
            }

            foreach (var assetPath in NormalizeAssetPaths(changedAssetPaths))
            {
                if (!scanner.MatchesInputGlobs(assetPath, config.inputGlobs))
                {
                    fileScanResults.Remove(assetPath);
                    continue;
                }

                var absolutePath = Path.Combine(currentProjectRoot, assetPath.Replace('/', Path.DirectorySeparatorChar));
                if (!File.Exists(absolutePath))
                {
                    fileScanResults.Remove(assetPath);
                    continue;
                }

                fileScanResults[assetPath] = scanner.ScanMatchedFile(currentProjectRoot, assetPath);
            }

            return BuildAggregatedResult();
        }

        private void EnsureInitialized(string currentProjectRoot, TailwindUssConfig config)
        {
            var nextConfigSignature = CreateConfigSignature(config);
            if (string.Equals(projectRoot, currentProjectRoot, StringComparison.Ordinal) &&
                string.Equals(configSignature, nextConfigSignature, StringComparison.Ordinal) &&
                fileScanResults.Count > 0)
            {
                return;
            }

            Reset();
            projectRoot = currentProjectRoot;
            configSignature = nextConfigSignature;
            UpdateFromFullScan(currentProjectRoot, config, scanner.Scan(currentProjectRoot, config.inputGlobs));
        }

        private UxmlScanResult BuildAggregatedResult()
        {
            var result = new UxmlScanResult();
            var nextClassAttributeId = 1;
            var matchedFiles = new List<string>(fileScanResults.Keys);
            matchedFiles.Sort(StringComparer.Ordinal);

            foreach (var matchedFile in matchedFiles)
            {
                result.MatchedFiles.Add(matchedFile);

                var fileResult = fileScanResults[matchedFile];
                foreach (var diagnostic in fileResult.Diagnostics)
                {
                    result.Diagnostics.Add(diagnostic);
                }

                var classAttributeIdMap = new Dictionary<int, int>();
                foreach (var occurrence in fileResult.Occurrences)
                {
                    if (!classAttributeIdMap.TryGetValue(occurrence.ClassAttributeId, out var remappedClassAttributeId))
                    {
                        remappedClassAttributeId = nextClassAttributeId++;
                        classAttributeIdMap.Add(occurrence.ClassAttributeId, remappedClassAttributeId);
                    }

                    result.Occurrences.Add(new UxmlTokenOccurrence(
                        occurrence.RelativeFilePath,
                        occurrence.LineNumber,
                        occurrence.ElementName,
                        occurrence.OriginalToken,
                        occurrence.VariantChain,
                        occurrence.BaseToken,
                        remappedClassAttributeId));
                }
            }

            return result;
        }

        private static IEnumerable<string> NormalizeAssetPaths(IEnumerable<string> assetPaths)
        {
            if (assetPaths == null)
            {
                yield break;
            }

            foreach (var assetPath in assetPaths)
            {
                if (string.IsNullOrWhiteSpace(assetPath) ||
                    !assetPath.EndsWith(".uxml", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                yield return assetPath.Replace('\\', '/');
            }
        }

        private static string CreateConfigSignature(TailwindUssConfig config)
        {
            return JsonConvert.SerializeObject(config);
        }
    }
}
