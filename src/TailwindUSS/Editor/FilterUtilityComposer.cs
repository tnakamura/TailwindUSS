using System.Collections.Generic;

namespace TailwindUSS.Editor
{
    internal sealed class FilterUtilityComposer
    {
        private static readonly string[] FilterOrder =
        {
            "blur",
            "grayscale",
            "invert",
            "sepia",
            "contrast",
            "hue-rotate"
        };

        public IList<ResolvedUtility> Compose(IList<ResolvedTokenOccurrence> filterOccurrences, IList<TailwindUssDiagnostic> diagnostics)
        {
            var compositeUtilities = new Dictionary<string, ResolvedUtility>(System.StringComparer.Ordinal);
            var occurrencesByClassAttribute = new Dictionary<int, List<ResolvedTokenOccurrence>>();

            foreach (var filterOccurrence in filterOccurrences)
            {
                List<ResolvedTokenOccurrence> occurrences;
                if (!occurrencesByClassAttribute.TryGetValue(filterOccurrence.Occurrence.ClassAttributeId, out occurrences))
                {
                    occurrences = new List<ResolvedTokenOccurrence>();
                    occurrencesByClassAttribute.Add(filterOccurrence.Occurrence.ClassAttributeId, occurrences);
                }

                occurrences.Add(filterOccurrence);
            }

            foreach (var pair in occurrencesByClassAttribute)
            {
                ComposeForClassAttribute(pair.Value, diagnostics, compositeUtilities);
            }

            return new List<ResolvedUtility>(compositeUtilities.Values);
        }

        private static void ComposeForClassAttribute(
            IList<ResolvedTokenOccurrence> occurrences,
            IList<TailwindUssDiagnostic> diagnostics,
            IDictionary<string, ResolvedUtility> compositeUtilities)
        {
            var groupsBySelectorSuffix = new Dictionary<string, List<ResolvedTokenOccurrence>>(System.StringComparer.Ordinal);
            foreach (var occurrence in occurrences)
            {
                List<ResolvedTokenOccurrence> groupedOccurrences;
                if (!groupsBySelectorSuffix.TryGetValue(occurrence.Utility.SelectorSuffix, out groupedOccurrences))
                {
                    groupedOccurrences = new List<ResolvedTokenOccurrence>();
                    groupsBySelectorSuffix.Add(occurrence.Utility.SelectorSuffix, groupedOccurrences);
                }

                groupedOccurrences.Add(occurrence);
            }

            var baseFilters = SelectEffectiveFilters(GetGroup(groupsBySelectorSuffix, string.Empty), diagnostics);
            AddCompositeUtility(baseFilters, string.Empty, compositeUtilities);

            foreach (var pair in groupsBySelectorSuffix)
            {
                if (pair.Key.Length == 0)
                {
                    continue;
                }

                var effectiveFilters = new Dictionary<string, ResolvedTokenOccurrence>(baseFilters, System.StringComparer.Ordinal);
                foreach (var variantFilter in SelectEffectiveFilters(pair.Value, diagnostics))
                {
                    effectiveFilters[variantFilter.Key] = variantFilter.Value;
                }

                AddCompositeUtility(effectiveFilters, pair.Key, compositeUtilities);
            }
        }

        private static IList<ResolvedTokenOccurrence> GetGroup(IDictionary<string, List<ResolvedTokenOccurrence>> groups, string selectorSuffix)
        {
            List<ResolvedTokenOccurrence> occurrences;
            return groups.TryGetValue(selectorSuffix, out occurrences)
                ? occurrences
                : new List<ResolvedTokenOccurrence>();
        }

        private static IDictionary<string, ResolvedTokenOccurrence> SelectEffectiveFilters(
            IList<ResolvedTokenOccurrence> occurrences,
            IList<TailwindUssDiagnostic> diagnostics)
        {
            var selected = new Dictionary<string, ResolvedTokenOccurrence>(System.StringComparer.Ordinal);
            foreach (var occurrence in occurrences)
            {
                var family = occurrence.Utility.FilterContribution.Family;
                ResolvedTokenOccurrence previous;
                if (selected.TryGetValue(family, out previous))
                {
                    diagnostics.Add(new TailwindUssDiagnostic(
                        DiagnosticSeverity.Warning,
                        TokenIssueKind.Duplicate,
                        string.Format("Duplicate filter family '{0}' in class attribute. Using last token '{1}'.", family, occurrence.Occurrence.OriginalToken),
                        occurrence.Occurrence.RelativeFilePath,
                        occurrence.Occurrence.LineNumber,
                        occurrence.Occurrence.ElementName,
                        occurrence.Occurrence.OriginalToken));
                }

                selected[family] = occurrence;
            }

            return selected;
        }

        private static void AddCompositeUtility(
            IDictionary<string, ResolvedTokenOccurrence> effectiveFilters,
            string selectorSuffix,
            IDictionary<string, ResolvedUtility> compositeUtilities)
        {
            if (effectiveFilters.Count == 0)
            {
                return;
            }

            var filterBuilder = new System.Text.StringBuilder();
            var classNames = new List<string>();

            for (var i = 0; i < FilterOrder.Length; i++)
            {
                ResolvedTokenOccurrence occurrence;
                if (!effectiveFilters.TryGetValue(FilterOrder[i], out occurrence))
                {
                    continue;
                }

                classNames.Add(occurrence.Utility.Token);
                if (filterBuilder.Length > 0)
                {
                    filterBuilder.Append(' ');
                }

                filterBuilder.Append(occurrence.Utility.FilterContribution.Function);
            }

            if (classNames.Count == 0 || filterBuilder.Length == 0)
            {
                return;
            }

            var selector = UssEmitter.BuildSelector(classNames, selectorSuffix);
            compositeUtilities[selector] = new ResolvedUtility(
                selector,
                new[] { new StyleDeclaration("filter", filterBuilder.ToString()) },
                selectorOverride: selector);
        }
    }
}
