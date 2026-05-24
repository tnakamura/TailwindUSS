using System;
using System.Collections.Generic;

namespace TailwindUSS.Editor
{
    /// <summary>
    /// Represents the tailwind uss config.
    /// </summary>
    [Serializable]
    internal sealed class TailwindUssConfig
    {
        /// <summary>
        /// The input globs.
        /// </summary>
        public string[] inputGlobs = { "Assets/**/*.uxml" };
        /// <summary>
        /// The output uss path.
        /// </summary>
        public string outputUssPath = "Assets/Generated/TailwindUSS.generated.uss";
        /// <summary>
        /// The auto attach generated uss.
        /// </summary>
        public bool autoAttachGeneratedUss;
        /// <summary>
        /// The auto generate on uxml save.
        /// </summary>
        public bool autoGenerateOnUxmlSave;
        /// <summary>
        /// The theme.
        /// </summary>
        public TailwindUssTheme theme;

        /// <summary>
        /// Creates a new instance with default configuration values.
        /// </summary>
        public static TailwindUssConfig CreateDefault()
        {
            return new TailwindUssConfig();
        }
    }

    /// <summary>
    /// Represents the tailwind uss theme.
    /// </summary>
    [Serializable]
    internal sealed class TailwindUssTheme
    {
        /// <summary>
        /// The colors.
        /// </summary>
        public Dictionary<string, string> colors;
        /// <summary>
        /// The spacing.
        /// </summary>
        public Dictionary<string, string> spacing;
        /// <summary>
        /// The font sizes.
        /// </summary>
        public Dictionary<string, string> fontSizes;
        /// <summary>
        /// The fonts.
        /// </summary>
        public Dictionary<string, string> fonts;
        /// <summary>
        /// The background images.
        /// </summary>
        public Dictionary<string, string> backgroundImages;

        /// <summary>
        /// Creates a merged theme by combining the provided theme with built-in defaults.
        /// </summary>
        public static TailwindUssTheme CreateMerged(TailwindUssTheme theme)
        {
            var merged = new TailwindUssTheme
            {
                colors = CreateDefaultColors(),
                spacing = CreateDefaultSpacing(),
                fontSizes = CreateDefaultFontSizes(),
                fonts = new Dictionary<string, string>(StringComparer.Ordinal),
                backgroundImages = new Dictionary<string, string>(StringComparer.Ordinal)
            };

            if (theme == null)
            {
                return merged;
            }

            MergeInto(merged.colors, theme.colors);
            MergeInto(merged.spacing, theme.spacing);
            MergeInto(merged.fontSizes, theme.fontSizes);
            MergeInto(merged.fonts, theme.fonts);
            MergeInto(merged.backgroundImages, theme.backgroundImages);
            return merged;
        }

        private static void MergeInto(IDictionary<string, string> target, IDictionary<string, string> source)
        {
            if (source == null)
            {
                return;
            }

            foreach (var pair in source)
            {
                target[pair.Key] = pair.Value;
            }
        }

        private static Dictionary<string, string> CreateDefaultSpacing()
        {
            return new Dictionary<string, string>(StringComparer.Ordinal)
            {
                { "0", "0px" },
                { "1", "4px" },
                { "2", "8px" },
                { "3", "12px" },
                { "4", "16px" },
                { "5", "20px" },
                { "6", "24px" },
                { "8", "32px" },
                { "10", "40px" },
                { "12", "48px" }
            };
        }

        private static Dictionary<string, string> CreateDefaultFontSizes()
        {
            return new Dictionary<string, string>(StringComparer.Ordinal)
            {
                { "xs", "12px" },
                { "sm", "14px" },
                { "base", "16px" },
                { "lg", "18px" },
                { "xl", "20px" },
                { "2xl", "24px" },
                { "3xl", "30px" },
                { "4xl", "36px" },
                { "5xl", "48px" },
                { "6xl", "60px" },
                { "7xl", "72px" },
                { "8xl", "96px" },
                { "9xl", "128px" }
            };
        }

        private static Dictionary<string, string> CreateDefaultColors()
        {
            return new Dictionary<string, string>(StringComparer.Ordinal)
            {
                { "white", "#FFFFFF" },
                { "black", "#000000" },
                { "gray-100", "#F3F4F6" },
                { "gray-300", "#D1D5DB" },
                { "gray-500", "#6B7280" },
                { "gray-700", "#374151" },
                { "gray-900", "#111827" },
                { "slate-100", "#F1F5F9" },
                { "slate-300", "#CBD5E1" },
                { "slate-500", "#64748B" },
                { "slate-700", "#334155" },
                { "slate-900", "#0F172A" },
                { "zinc-100", "#F4F4F5" },
                { "zinc-300", "#D4D4D8" },
                { "zinc-500", "#71717A" },
                { "zinc-700", "#3F3F46" },
                { "zinc-900", "#18181B" },
                { "neutral-100", "#F5F5F5" },
                { "neutral-300", "#D4D4D4" },
                { "neutral-500", "#737373" },
                { "neutral-700", "#404040" },
                { "neutral-900", "#171717" },
                { "stone-100", "#F5F5F4" },
                { "stone-300", "#D6D3D1" },
                { "stone-500", "#78716C" },
                { "stone-700", "#44403C" },
                { "stone-900", "#1C1917" },
                { "blue-500", "#3B82F6" },
                { "red-500", "#EF4444" },
                { "green-500", "#22C55E" },
                { "yellow-500", "#EAB308" },
                { "emerald-100", "#D1FAE5" },
                { "emerald-300", "#6EE7B7" },
                { "emerald-500", "#10B981" },
                { "emerald-700", "#047857" },
                { "emerald-900", "#064E3B" },
                { "sky-100", "#E0F2FE" },
                { "sky-300", "#7DD3FC" },
                { "sky-500", "#0EA5E9" },
                { "sky-700", "#0369A1" },
                { "sky-900", "#0C4A6E" },
                { "indigo-100", "#E0E7FF" },
                { "indigo-300", "#A5B4FC" },
                { "indigo-500", "#6366F1" },
                { "indigo-700", "#4338CA" },
                { "indigo-900", "#312E81" },
                { "pink-100", "#FCE7F3" },
                { "pink-300", "#F9A8D4" },
                { "pink-500", "#EC4899" },
                { "pink-700", "#BE185D" },
                { "pink-900", "#831843" }
            };
        }
    }
}
