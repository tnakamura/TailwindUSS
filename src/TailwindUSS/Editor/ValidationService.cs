using System;
using System.Collections.Generic;
using UnityEngine;

namespace TailwindUSS.Editor
{
    /// <summary>
    /// Represents the validation service.
    /// </summary>
    internal sealed class ValidationService
    {
        private readonly UxmlScanner scanner = new UxmlScanner();
        private readonly FilterUtilityComposer filterUtilityComposer = new FilterUtilityComposer();

        /// <summary>
        /// Validates utility tokens found in UXML files and reports diagnostics.
        /// </summary>
        public CommandResult Validate()
        {
            var result = new CommandResult();
            TailwindUssConfig config;
            string errorMessage;
            bool usedDefaultConfig;
            if (!ConfigLoader.TryLoad(out config, out errorMessage, out usedDefaultConfig))
            {
                GenerationService.LogDiagnostic(new TailwindUssDiagnostic(
                    DiagnosticSeverity.Error,
                    null,
                    string.Format("Failed to load '{0}': {1}", ConfigLoader.FileName, errorMessage),
                    ConfigLoader.FileName,
                    0,
                    string.Empty,
                    string.Empty));

                result.ErrorCount = 1;
                return result;
            }

            if (usedDefaultConfig)
            {
                Debug.LogWarning(string.Format("TailwindUSS config was not found. Using in-memory defaults; create '{0}' to persist settings.", ConfigLoader.FileName));
            }

            var scanResult = scanner.Scan(ConfigLoader.GetProjectRoot(), config.inputGlobs);
            var resolver = new UtilityResolver(config.theme);
            var supportedCount = 0;
            var filterOccurrences = new List<ResolvedTokenOccurrence>();

            foreach (var occurrence in scanResult.Occurrences)
            {
                ResolvedUtility utility;
                string resolveError;
                var status = resolver.TryResolve(occurrence, out utility, out resolveError);
                switch (status)
                {
                    case ResolveStatus.Supported:
                        supportedCount++;
                        if (utility.IsFilterUtility)
                        {
                            filterOccurrences.Add(new ResolvedTokenOccurrence(occurrence, utility));
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
                            string.Format("Unsupported utility token '{0}'.", occurrence.OriginalToken),
                            occurrence.RelativeFilePath,
                            occurrence.LineNumber,
                            occurrence.ElementName,
                            occurrence.OriginalToken));
                        break;
                }
            }

            filterUtilityComposer.Compose(filterOccurrences, scanResult.Diagnostics);

            foreach (var diagnostic in scanResult.Diagnostics)
            {
                GenerationService.LogDiagnostic(diagnostic);
            }

            result.GeneratedUtilityCount = supportedCount;
            result.WarningCount = CountDiagnostics(scanResult, DiagnosticSeverity.Warning);
            result.ErrorCount = CountDiagnostics(scanResult, DiagnosticSeverity.Error);

            Debug.Log(string.Format(
                "TailwindUSS validation finished. Supported tokens: {0}, Warnings: {1}, Errors: {2}",
                result.GeneratedUtilityCount,
                result.WarningCount,
                result.ErrorCount));

            return result;
        }

        private static int CountDiagnostics(UxmlScanResult scanResult, DiagnosticSeverity severity)
        {
            var count = 0;
            foreach (var diagnostic in scanResult.Diagnostics)
            {
                if (diagnostic.Severity == severity)
                {
                    count++;
                }
            }

            return count;
        }
    }
}
