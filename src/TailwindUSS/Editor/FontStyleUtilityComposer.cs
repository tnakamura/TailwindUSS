using System;
using System.Collections.Generic;

namespace TailwindUSS.Editor
{
    /// <summary>
    /// Represents the font style utility composer.
    /// </summary>
    internal sealed class FontStyleUtilityComposer
    {
        private const string WeightFamily = "weight";
        private const string SlantFamily = "slant";

        /// <summary>
        /// Composes font style utilities so combined bold and italic states map to Unity's single font style property.
        /// </summary>
        public IList<ResolvedUtility> Compose(IList<ResolvedTokenOccurrence> fontStyleOccurrences)
        {
            var compositeUtilities = new Dictionary<string, ResolvedUtility>(StringComparer.Ordinal);
            var occurrencesByClassAttribute = new Dictionary<int, List<ResolvedTokenOccurrence>>();

            foreach (var fontStyleOccurrence in fontStyleOccurrences)
            {
                List<ResolvedTokenOccurrence> occurrences;
                if (!occurrencesByClassAttribute.TryGetValue(fontStyleOccurrence.Occurrence.ClassAttributeId, out occurrences))
                {
                    occurrences = new List<ResolvedTokenOccurrence>();
                    occurrencesByClassAttribute.Add(fontStyleOccurrence.Occurrence.ClassAttributeId, occurrences);
                }

                occurrences.Add(fontStyleOccurrence);
            }

            foreach (var pair in occurrencesByClassAttribute)
            {
                ComposeForClassAttribute(pair.Value, compositeUtilities);
            }

            return new List<ResolvedUtility>(compositeUtilities.Values);
        }

        private static void ComposeForClassAttribute(
            IList<ResolvedTokenOccurrence> occurrences,
            IDictionary<string, ResolvedUtility> compositeUtilities)
        {
            var groupsBySelectorSuffix = new Dictionary<string, List<ResolvedTokenOccurrence>>(StringComparer.Ordinal);
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

            var baseStyles = SelectEffectiveStyles(GetGroup(groupsBySelectorSuffix, string.Empty));
            AddCompositeUtility(baseStyles, string.Empty, compositeUtilities);

            foreach (var pair in groupsBySelectorSuffix)
            {
                if (pair.Key.Length == 0)
                {
                    continue;
                }

                var effectiveStyles = new Dictionary<string, ResolvedTokenOccurrence>(baseStyles, StringComparer.Ordinal);
                foreach (var style in SelectEffectiveStyles(pair.Value))
                {
                    effectiveStyles[style.Key] = style.Value;
                }

                AddCompositeUtility(effectiveStyles, pair.Key, compositeUtilities);
            }
        }

        private static IList<ResolvedTokenOccurrence> GetGroup(IDictionary<string, List<ResolvedTokenOccurrence>> groups, string selectorSuffix)
        {
            List<ResolvedTokenOccurrence> occurrences;
            return groups.TryGetValue(selectorSuffix, out occurrences)
                ? occurrences
                : new List<ResolvedTokenOccurrence>();
        }

        private static IDictionary<string, ResolvedTokenOccurrence> SelectEffectiveStyles(IList<ResolvedTokenOccurrence> occurrences)
        {
            var selected = new Dictionary<string, ResolvedTokenOccurrence>(StringComparer.Ordinal);
            foreach (var occurrence in occurrences)
            {
                var family = GetFamily(occurrence);
                if (family == null)
                {
                    continue;
                }

                selected[family] = occurrence;
            }

            return selected;
        }

        private static string GetFamily(ResolvedTokenOccurrence occurrence)
        {
            if (occurrence.Occurrence.BaseToken == "italic" || occurrence.Occurrence.BaseToken == "not-italic")
            {
                return SlantFamily;
            }

            if (occurrence.Occurrence.BaseToken.StartsWith("font-", StringComparison.Ordinal)
                && occurrence.Utility.Declarations.Count == 1
                && occurrence.Utility.Declarations[0].PropertyName == "-unity-font-style")
            {
                return WeightFamily;
            }

            return null;
        }

        private static void AddCompositeUtility(
            IDictionary<string, ResolvedTokenOccurrence> effectiveStyles,
            string selectorSuffix,
            IDictionary<string, ResolvedUtility> compositeUtilities)
        {
            ResolvedTokenOccurrence weightOccurrence;
            ResolvedTokenOccurrence slantOccurrence;
            if (!effectiveStyles.TryGetValue(WeightFamily, out weightOccurrence)
                || !effectiveStyles.TryGetValue(SlantFamily, out slantOccurrence))
            {
                return;
            }

            var combinedValue = CombineFontStyle(
                weightOccurrence.Utility.Declarations[0].Value,
                slantOccurrence.Utility.Declarations[0].Value);

            var selector = UssEmitter.BuildSelector(
                new[] { weightOccurrence.Utility.Token, slantOccurrence.Utility.Token },
                selectorSuffix);

            compositeUtilities[selector] = new ResolvedUtility(
                selector,
                new[] { new StyleDeclaration("-unity-font-style", combinedValue) },
                selectorOverride: selector);
        }

        private static string CombineFontStyle(string weightValue, string slantValue)
        {
            if (weightValue == "bold")
            {
                return slantValue == "italic"
                    ? "bold-and-italic"
                    : "bold";
            }

            return slantValue == "italic"
                ? "italic"
                : "normal";
        }
    }
}