using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace TailwindUSS.Editor
{
    /// <summary>
    /// Represents the generation service.
    /// </summary>
    internal sealed class GenerationService
    {
        private static readonly IncrementalGenerationCache IncrementalCache = IncrementalGenerationCache.Shared;
        private readonly UxmlScanner scanner = new UxmlScanner();
        private readonly UssEmitter emitter = new UssEmitter();
        private readonly UxmlStyleReferenceUpdater styleReferenceUpdater = new UxmlStyleReferenceUpdater();
        private readonly FilterUtilityComposer filterUtilityComposer = new FilterUtilityComposer();
        private readonly FontStyleUtilityComposer fontStyleUtilityComposer = new FontStyleUtilityComposer();

        /// <summary>
        /// Generates USS output from utility tokens found in UXML files.
        /// </summary>
        public CommandResult Generate()
        {
            if (!TryLoadConfig(out var config, out var result, out var projectRoot))
            {
                return result;
            }

            var scanResult = scanner.Scan(projectRoot, config.inputGlobs);
            IncrementalCache.UpdateFromFullScan(projectRoot, config, scanResult);
            return GenerateFromScan(projectRoot, config, scanResult);
        }

        internal CommandResult GenerateIncremental(params string[][] assetPathGroups)
        {
            if (!TryLoadConfig(out var config, out var result, out var projectRoot))
            {
                return result;
            }

            var changedAssetPaths = new List<string>();
            var deletedAssetPaths = new List<string>();
            AppendAssetPaths(changedAssetPaths, assetPathGroups, 0, 2);
            AppendAssetPaths(deletedAssetPaths, assetPathGroups, 1, 3);

            var scanResult = IncrementalCache.UpdateAndBuildScan(projectRoot, config, changedAssetPaths, deletedAssetPaths);
            return GenerateFromScan(projectRoot, config, scanResult);
        }

        /// <summary>
        /// Logs a diagnostic message to Unity's console with context information when available.
        /// </summary>
        internal static void LogDiagnostic(TailwindUssDiagnostic diagnostic)
        {
            var location = string.Empty;
            if (!string.IsNullOrEmpty(diagnostic.RelativeFilePath))
            {
                location = diagnostic.LineNumber > 0
                    ? string.Format("{0}:{1}", diagnostic.RelativeFilePath, diagnostic.LineNumber)
                    : diagnostic.RelativeFilePath;

                if (!string.IsNullOrEmpty(diagnostic.ElementName))
                {
                    location = string.Format("{0}, {1}", location, diagnostic.ElementName);
                }
            }

            var message = string.IsNullOrEmpty(location)
                ? diagnostic.Message
                : string.Format("{0} ({1})", diagnostic.Message, location);

            UnityEngine.Object context = null;
            if (!string.IsNullOrEmpty(diagnostic.RelativeFilePath) && diagnostic.RelativeFilePath.StartsWith("Assets/", StringComparison.Ordinal))
            {
                context = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(diagnostic.RelativeFilePath);
            }

            if (diagnostic.Severity == DiagnosticSeverity.Error)
            {
                Debug.LogError(message, context);
                return;
            }

            Debug.LogWarning(message, context);
        }

        internal static string FormatUnsupportedUtilityTokenMessage(string token, string resolveError)
        {
            if (string.IsNullOrEmpty(resolveError))
            {
                return string.Format("Unsupported utility token '{0}'.", token);
            }

            return string.Format("Unsupported utility token '{0}': {1}", token, resolveError);
        }

        private static string GetAbsoluteOutputPath(string projectRoot, string configuredOutputPath)
        {
            if (Path.IsPathRooted(configuredOutputPath))
            {
                return configuredOutputPath;
            }

            return Path.GetFullPath(Path.Combine(projectRoot, configuredOutputPath));
        }

        private static string NormalizeAssetPath(string projectRoot, string absolutePath)
        {
            if (!absolutePath.StartsWith(projectRoot, StringComparison.Ordinal))
            {
                return absolutePath.Replace('\\', '/');
            }

            var relativePath = absolutePath.Substring(projectRoot.Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            return relativePath.Replace('\\', '/');
        }

        private static int CountDiagnostics(IEnumerable<TailwindUssDiagnostic> diagnostics, DiagnosticSeverity severity)
        {
            var count = 0;
            foreach (var diagnostic in diagnostics)
            {
                if (diagnostic.Severity == severity)
                {
                    count++;
                }
            }

            return count;
        }

        private static bool IsFontStyleUtility(ResolvedUtility utility)
        {
            return utility.Declarations.Count == 1
                && utility.Declarations[0].PropertyName == "-unity-font-style";
        }

        private static void AppendAssetPaths(ICollection<string> target, string[][] assetPathGroups, params int[] indices)
        {
            foreach (var index in indices)
            {
                if (index < 0 || index >= assetPathGroups.Length || assetPathGroups[index] == null)
                {
                    continue;
                }

                foreach (var assetPath in assetPathGroups[index])
                {
                    target.Add(assetPath);
                }
            }
        }

        private static bool TryLoadConfig(out TailwindUssConfig config, out CommandResult result, out string projectRoot)
        {
            result = new CommandResult();
            projectRoot = null;
            string errorMessage;
            bool usedDefaultConfig;
            if (!ConfigLoader.TryLoad(out config, out errorMessage, out usedDefaultConfig))
            {
                LogDiagnostic(new TailwindUssDiagnostic(
                    DiagnosticSeverity.Error,
                    null,
                    string.Format("Failed to load '{0}': {1}", ConfigLoader.FileName, errorMessage),
                    ConfigLoader.FileName,
                    0,
                    string.Empty,
                    string.Empty));

                result.ErrorCount = 1;
                return false;
            }

            if (usedDefaultConfig)
            {
                Debug.LogWarning(string.Format("TailwindUSS config was not found. Using in-memory defaults; create '{0}' to persist settings.", ConfigLoader.FileName));
            }

            projectRoot = ConfigLoader.GetProjectRoot();
            return true;
        }

        private CommandResult GenerateFromScan(string projectRoot, TailwindUssConfig config, UxmlScanResult scanResult)
        {
            var result = new CommandResult();
            var resolver = new UtilityResolver(config.theme);
            var utilities = new Dictionary<string, ResolvedUtility>(StringComparer.Ordinal);
            var filterOccurrences = new List<ResolvedTokenOccurrence>();
            var fontStyleOccurrences = new List<ResolvedTokenOccurrence>();

            foreach (var occurrence in scanResult.Occurrences)
            {
                ResolvedUtility utility;
                string resolveError;
                var status = resolver.TryResolve(occurrence, out utility, out resolveError);
                switch (status)
                {
                    case ResolveStatus.Supported:
                        if (utility.IsFilterUtility)
                        {
                            filterOccurrences.Add(new ResolvedTokenOccurrence(occurrence, utility));
                        }
                        else if (IsFontStyleUtility(utility))
                        {
                            fontStyleOccurrences.Add(new ResolvedTokenOccurrence(occurrence, utility));

                            if (!utilities.ContainsKey(occurrence.OriginalToken))
                            {
                                utilities.Add(occurrence.OriginalToken, utility);
                            }
                        }
                        else if (!utilities.ContainsKey(occurrence.OriginalToken))
                        {
                            utilities.Add(occurrence.OriginalToken, utility);
                        }

                        break;
                    case ResolveStatus.UnsupportedVariant:
                        scanResult.Diagnostics.Add(new TailwindUssDiagnostic(
                            DiagnosticSeverity.Warning,
                            TokenIssueKind.UnsupportedVariant,
                            string.Format("Unsupported variant in utility token '{0}': {1}", occurrence.OriginalToken, resolveError),
                            occurrence.RelativeFilePath,
                            occurrence.LineNumber,
                            occurrence.ElementName,
                            occurrence.OriginalToken));
                        break;
                    case ResolveStatus.InvalidValue:
                        scanResult.Diagnostics.Add(new TailwindUssDiagnostic(
                            DiagnosticSeverity.Warning,
                            TokenIssueKind.InvalidValue,
                            string.Format("Invalid utility token '{0}': {1}", occurrence.OriginalToken, resolveError),
                            occurrence.RelativeFilePath,
                            occurrence.LineNumber,
                            occurrence.ElementName,
                            occurrence.OriginalToken));
                        break;
                    default:
                        scanResult.Diagnostics.Add(new TailwindUssDiagnostic(
                            DiagnosticSeverity.Warning,
                            TokenIssueKind.Unsupported,
                            FormatUnsupportedUtilityTokenMessage(occurrence.OriginalToken, resolveError),
                            occurrence.RelativeFilePath,
                            occurrence.LineNumber,
                            occurrence.ElementName,
                            occurrence.OriginalToken));
                        break;
                }
            }

            try
            {
                var compositeFilterUtilities = filterUtilityComposer.Compose(filterOccurrences, scanResult.Diagnostics);
                var compositeFontStyleUtilities = fontStyleUtilityComposer.Compose(fontStyleOccurrences);
                var emittedUtilities = new List<ResolvedUtility>(utilities.Values);
                emittedUtilities.AddRange(compositeFilterUtilities);
                emittedUtilities.AddRange(compositeFontStyleUtilities);
                var outputAbsolutePath = GetAbsoluteOutputPath(projectRoot, config.outputUssPath);
                var outputDirectory = Path.GetDirectoryName(outputAbsolutePath);
                if (!string.IsNullOrEmpty(outputDirectory))
                {
                    Directory.CreateDirectory(outputDirectory);
                }

                File.WriteAllText(outputAbsolutePath, emitter.Emit(emittedUtilities));
                result.OutputAssetPath = NormalizeAssetPath(projectRoot, outputAbsolutePath);
                result.GeneratedUtilityCount = emittedUtilities.Count;
            }
            catch (Exception exception)
            {
                scanResult.Diagnostics.Add(new TailwindUssDiagnostic(
                    DiagnosticSeverity.Error,
                    null,
                    string.Format("Failed to write USS output: {0}", exception.Message),
                    config.outputUssPath,
                    0,
                    string.Empty,
                    string.Empty));
            }

            if (config.autoAttachGeneratedUss && result.OutputAssetPath != null)
            {
                styleReferenceUpdater.EnsureStyleReference(projectRoot, scanResult.MatchedFiles, result.OutputAssetPath, scanResult.Diagnostics);
            }

            foreach (var diagnostic in scanResult.Diagnostics)
            {
                LogDiagnostic(diagnostic);
            }

            AssetDatabase.Refresh();

            if (result.GeneratedUtilityCount == 0)
            {
                result.GeneratedUtilityCount = utilities.Count;
            }

            result.WarningCount = CountDiagnostics(scanResult.Diagnostics, DiagnosticSeverity.Warning);
            result.ErrorCount = CountDiagnostics(scanResult.Diagnostics, DiagnosticSeverity.Error);

            Debug.Log(string.Format(
                "TailwindUSS generation finished. Generated: {0}, Warnings: {1}, Errors: {2}, Output: {3}",
                result.GeneratedUtilityCount,
                result.WarningCount,
                result.ErrorCount,
                result.OutputAssetPath ?? config.outputUssPath));

            return result;
        }
    }
}
