using System;
using System.Collections.Generic;

namespace TailwindUSS.Editor
{
    internal sealed class UtilityResolver
    {
        private delegate bool UtilityHandler(string token, out ResolvedUtility utility, out string errorMessage);

        private static readonly IDictionary<string, string> SpacingScale = new Dictionary<string, string>(StringComparer.Ordinal)
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

        private static readonly IDictionary<string, string> FontSizes = new Dictionary<string, string>(StringComparer.Ordinal)
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

        private static readonly IDictionary<string, string> Colors = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            { "white", "#FFFFFF" },
            { "black", "#000000" },
            { "gray-100", "#F3F4F6" },
            { "gray-300", "#D1D5DB" },
            { "gray-500", "#6B7280" },
            { "gray-700", "#374151" },
            { "gray-900", "#111827" },
            { "blue-500", "#3B82F6" },
            { "red-500", "#EF4444" },
            { "green-500", "#22C55E" },
            { "yellow-500", "#EAB308" }
        };

        private static readonly IDictionary<string, string> ZIndexScale = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            { "0", "0" },
            { "10", "10" },
            { "20", "20" },
            { "30", "30" },
            { "40", "40" },
            { "50", "50" },
            { "auto", "auto" }
        };

        private static readonly IDictionary<string, string> OpacityScale = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            { "0", "0" },
            { "5", "0.05" },
            { "10", "0.1" },
            { "20", "0.2" },
            { "25", "0.25" },
            { "30", "0.3" },
            { "40", "0.4" },
            { "50", "0.5" },
            { "60", "0.6" },
            { "70", "0.7" },
            { "75", "0.75" },
            { "80", "0.8" },
            { "90", "0.9" },
            { "95", "0.95" },
            { "100", "1" }
        };

        private static readonly IDictionary<string, string> OrderScale = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            { "0", "0" },
            { "1", "1" },
            { "2", "2" },
            { "3", "3" },
            { "4", "4" },
            { "5", "5" },
            { "6", "6" },
            { "7", "7" },
            { "8", "8" },
            { "9", "9" },
            { "10", "10" },
            { "11", "11" },
            { "12", "12" }
        };

        private static readonly IDictionary<string, string> TrackingScale = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            { "tighter", "-0.05em" },
            { "tight", "-0.025em" },
            { "normal", "0em" },
            { "wide", "0.025em" },
            { "wider", "0.05em" },
            { "widest", "0.1em" }
        };

        private static readonly IDictionary<string, string> LeadingScale = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            { "3", "12px" },
            { "4", "16px" },
            { "5", "20px" },
            { "6", "24px" },
            { "7", "28px" },
            { "8", "32px" },
            { "9", "36px" },
            { "10", "40px" }
        };

        private static readonly IDictionary<string, Func<string, ResolvedUtility>> FixedUtilities =
            new Dictionary<string, Func<string, ResolvedUtility>>(StringComparer.Ordinal)
            {
                { "flex", token => Create(token, new StyleDeclaration("display", "flex")) },
                { "hidden", token => Create(token, new StyleDeclaration("display", "none")) },
                { "flex-row", token => Create(token, new StyleDeclaration("flex-direction", "row")) },
                { "flex-col", token => Create(token, new StyleDeclaration("flex-direction", "column")) },
                { "grow", token => Create(token, new StyleDeclaration("flex-grow", "1")) },
                { "shrink", token => Create(token, new StyleDeclaration("flex-shrink", "1")) },
                { "items-start", token => Create(token, new StyleDeclaration("align-items", "flex-start")) },
                { "items-center", token => Create(token, new StyleDeclaration("align-items", "center")) },
                { "items-end", token => Create(token, new StyleDeclaration("align-items", "flex-end")) },
                { "items-stretch", token => Create(token, new StyleDeclaration("align-items", "stretch")) },
                { "justify-start", token => Create(token, new StyleDeclaration("justify-content", "flex-start")) },
                { "justify-center", token => Create(token, new StyleDeclaration("justify-content", "center")) },
                { "justify-end", token => Create(token, new StyleDeclaration("justify-content", "flex-end")) },
                { "justify-between", token => Create(token, new StyleDeclaration("justify-content", "space-between")) },
                { "justify-around", token => Create(token, new StyleDeclaration("justify-content", "space-around")) },
                { "justify-evenly", token => Create(token, new StyleDeclaration("justify-content", "space-evenly")) },
                { "font-normal", token => Create(token, new StyleDeclaration("-unity-font-style", "normal")) },
                { "font-bold", token => Create(token, new StyleDeclaration("-unity-font-style", "bold")) },
                { "italic", token => Create(token, new StyleDeclaration("-unity-font-style", "italic")) },
                { "not-italic", token => Create(token, new StyleDeclaration("-unity-font-style", "normal")) },
                { "text-left", token => Create(token, new StyleDeclaration("-unity-text-align", "middle-left")) },
                { "text-center", token => Create(token, new StyleDeclaration("-unity-text-align", "middle-center")) },
                { "text-right", token => Create(token, new StyleDeclaration("-unity-text-align", "middle-right")) },
                { "text-justify", token => Create(token, new StyleDeclaration("-unity-text-align", "middle-left")) },
                { "whitespace-normal", token => Create(token, new StyleDeclaration("white-space", "normal")) },
                { "whitespace-nowrap", token => Create(token, new StyleDeclaration("white-space", "nowrap")) },
                { "uppercase", token => Create(token, new StyleDeclaration("text-transform", "uppercase")) },
                { "lowercase", token => Create(token, new StyleDeclaration("text-transform", "lowercase")) },
                { "capitalize", token => Create(token, new StyleDeclaration("text-transform", "capitalize")) },
                { "normal-case", token => Create(token, new StyleDeclaration("text-transform", "none")) },
                { "truncate", token => Create(token, new[]
                    {
                        new StyleDeclaration("overflow", "hidden"),
                        new StyleDeclaration("text-overflow", "ellipsis"),
                        new StyleDeclaration("white-space", "nowrap")
                    }) },
                { "text-ellipsis", token => Create(token, new StyleDeclaration("text-overflow", "ellipsis")) },
                { "text-clip", token => Create(token, new StyleDeclaration("text-overflow", "clip")) },
                { "break-normal", token => Create(token, new StyleDeclaration("word-break", "normal")) },
                { "break-all", token => Create(token, new StyleDeclaration("word-break", "break-all")) },
                { "relative", token => Create(token, new StyleDeclaration("position", "relative")) },
                { "absolute", token => Create(token, new StyleDeclaration("position", "absolute")) },
                { "flex-wrap", token => Create(token, new StyleDeclaration("flex-wrap", "wrap")) },
                { "flex-nowrap", token => Create(token, new StyleDeclaration("flex-wrap", "nowrap")) },
                { "flex-wrap-reverse", token => Create(token, new StyleDeclaration("flex-wrap", "wrap-reverse")) },
                { "self-auto", token => Create(token, new StyleDeclaration("align-self", "auto")) },
                { "self-start", token => Create(token, new StyleDeclaration("align-self", "flex-start")) },
                { "self-end", token => Create(token, new StyleDeclaration("align-self", "flex-end")) },
                { "self-center", token => Create(token, new StyleDeclaration("align-self", "center")) },
                { "self-stretch", token => Create(token, new StyleDeclaration("align-self", "stretch")) },
                { "border", token => CreateBox(token, BorderWidthProperties, "1px") },
                { "border-0", token => CreateBox(token, BorderWidthProperties, "0px") },
                { "border-2", token => CreateBox(token, BorderWidthProperties, "2px") },
                { "rounded-none", token => CreateBox(token, RadiusProperties, "0px") },
                { "rounded-sm", token => CreateBox(token, RadiusProperties, "2px") },
                { "rounded", token => CreateBox(token, RadiusProperties, "4px") },
                { "rounded-md", token => CreateBox(token, RadiusProperties, "6px") },
                { "rounded-lg", token => CreateBox(token, RadiusProperties, "8px") },
                { "rounded-full", token => CreateBox(token, RadiusProperties, "9999px") }
            };

        private static readonly UtilityHandler[] Handlers =
        {
            TryResolveFontSize,
            TryResolveSpacing,
            TryResolveInset,
            TryResolveOverflow,
            TryResolveZIndex,
            TryResolveOpacity,
            TryResolveSize,
            TryResolveBasis,
            TryResolveOrder,
            TryResolveGap,
            TryResolveTracking,
            TryResolveLeading,
            TryResolveColor
        };

        private static readonly KeyValuePair<string, string[]>[] SpacingDefinitions =
        {
            new KeyValuePair<string, string[]>("px-", new[] { "padding-left", "padding-right" }),
            new KeyValuePair<string, string[]>("py-", new[] { "padding-top", "padding-bottom" }),
            new KeyValuePair<string, string[]>("pt-", new[] { "padding-top" }),
            new KeyValuePair<string, string[]>("pr-", new[] { "padding-right" }),
            new KeyValuePair<string, string[]>("pb-", new[] { "padding-bottom" }),
            new KeyValuePair<string, string[]>("pl-", new[] { "padding-left" }),
            new KeyValuePair<string, string[]>("p-", new[] { "padding-top", "padding-right", "padding-bottom", "padding-left" }),
            new KeyValuePair<string, string[]>("mx-", new[] { "margin-left", "margin-right" }),
            new KeyValuePair<string, string[]>("my-", new[] { "margin-top", "margin-bottom" }),
            new KeyValuePair<string, string[]>("mt-", new[] { "margin-top" }),
            new KeyValuePair<string, string[]>("mr-", new[] { "margin-right" }),
            new KeyValuePair<string, string[]>("mb-", new[] { "margin-bottom" }),
            new KeyValuePair<string, string[]>("ml-", new[] { "margin-left" }),
            new KeyValuePair<string, string[]>("m-", new[] { "margin-top", "margin-right", "margin-bottom", "margin-left" })
        };

        private static readonly KeyValuePair<string, string>[] SizeDefinitions =
        {
            new KeyValuePair<string, string>("min-w-", "min-width"),
            new KeyValuePair<string, string>("min-h-", "min-height"),
            new KeyValuePair<string, string>("max-w-", "max-width"),
            new KeyValuePair<string, string>("max-h-", "max-height"),
            new KeyValuePair<string, string>("w-", "width"),
            new KeyValuePair<string, string>("h-", "height")
        };

        private static readonly KeyValuePair<string, string[]>[] InsetDefinitions =
        {
            new KeyValuePair<string, string[]>("inset-x-", new[] { "left", "right" }),
            new KeyValuePair<string, string[]>("inset-y-", new[] { "top", "bottom" }),
            new KeyValuePair<string, string[]>("inset-", new[] { "top", "right", "bottom", "left" }),
            new KeyValuePair<string, string[]>("top-", new[] { "top" }),
            new KeyValuePair<string, string[]>("right-", new[] { "right" }),
            new KeyValuePair<string, string[]>("bottom-", new[] { "bottom" }),
            new KeyValuePair<string, string[]>("left-", new[] { "left" })
        };

        private static readonly string[] BorderWidthProperties =
        {
            "border-top-width",
            "border-right-width",
            "border-bottom-width",
            "border-left-width"
        };

        private static readonly string[] RadiusProperties =
        {
            "border-top-left-radius",
            "border-top-right-radius",
            "border-bottom-right-radius",
            "border-bottom-left-radius"
        };

        public ResolveStatus TryResolve(string token, out ResolvedUtility utility, out string errorMessage)
        {
            utility = null;
            errorMessage = null;

            Func<string, ResolvedUtility> fixedFactory;
            if (FixedUtilities.TryGetValue(token, out fixedFactory))
            {
                utility = fixedFactory(token);
                return ResolveStatus.Supported;
            }

            foreach (var handler in Handlers)
            {
                if (!handler(token, out utility, out errorMessage))
                {
                    continue;
                }

                return errorMessage == null ? ResolveStatus.Supported : ResolveStatus.InvalidValue;
            }

            return ResolveStatus.Unsupported;
        }

        private static bool TryResolveFontSize(string token, out ResolvedUtility utility, out string errorMessage)
        {
            errorMessage = null;
            utility = null;
            if (!token.StartsWith("text-", StringComparison.Ordinal))
            {
                return false;
            }

            var key = token.Substring("text-".Length);
            string value;
            if (!FontSizes.TryGetValue(key, out value))
            {
                return false;
            }

            utility = Create(token, new StyleDeclaration("font-size", value));
            return true;
        }

        private static bool TryResolveSpacing(string token, out ResolvedUtility utility, out string errorMessage)
        {
            return TryResolveScaleBox(token, SpacingDefinitions, SpacingScale, "Unsupported spacing scale value.", out utility, out errorMessage);
        }

        private static bool TryResolveInset(string token, out ResolvedUtility utility, out string errorMessage)
        {
            return TryResolveScaleBox(token, InsetDefinitions, SpacingScale, "Unsupported inset scale value.", out utility, out errorMessage);
        }

        private static bool TryResolveOverflow(string token, out ResolvedUtility utility, out string errorMessage)
        {
            errorMessage = null;
            utility = null;
            if (!token.StartsWith("overflow-", StringComparison.Ordinal))
            {
                return false;
            }

            var key = token.Substring("overflow-".Length);
            switch (key)
            {
                case "hidden":
                case "visible":
                case "scroll":
                    utility = Create(token, new StyleDeclaration("overflow", key));
                    return true;
                default:
                    errorMessage = "Unsupported overflow value.";
                    return true;
            }
        }

        private static bool TryResolveZIndex(string token, out ResolvedUtility utility, out string errorMessage)
        {
            return TryResolveMappedSingleProperty(token, "z-", "z-index", ZIndexScale, "Unsupported z-index value.", out utility, out errorMessage);
        }

        private static bool TryResolveOpacity(string token, out ResolvedUtility utility, out string errorMessage)
        {
            return TryResolveMappedSingleProperty(token, "opacity-", "opacity", OpacityScale, "Unsupported opacity value.", out utility, out errorMessage);
        }

        private static bool TryResolveSize(string token, out ResolvedUtility utility, out string errorMessage)
        {
            utility = null;
            errorMessage = null;

            foreach (var pair in SizeDefinitions)
            {
                if (!token.StartsWith(pair.Key, StringComparison.Ordinal))
                {
                    continue;
                }

                return TryResolveMappedSinglePropertyCore(token.Substring(pair.Key.Length), token, pair.Value, SpacingScale, "Unsupported size scale value.", out utility, out errorMessage);
            }

            return false;
        }

        private static bool TryResolveBasis(string token, out ResolvedUtility utility, out string errorMessage)
        {
            return TryResolveMappedSingleProperty(token, "basis-", "flex-basis", SpacingScale, "Unsupported size scale value.", out utility, out errorMessage);
        }

        private static bool TryResolveOrder(string token, out ResolvedUtility utility, out string errorMessage)
        {
            return TryResolveMappedSingleProperty(token, "order-", "order", OrderScale, "Unsupported order value.", out utility, out errorMessage);
        }

        private static bool TryResolveGap(string token, out ResolvedUtility utility, out string errorMessage)
        {
            if (TryResolveMappedSingleProperty(token, "gap-x-", "column-gap", SpacingScale, "Unsupported spacing scale value.", out utility, out errorMessage))
            {
                return true;
            }

            if (TryResolveMappedSingleProperty(token, "gap-y-", "row-gap", SpacingScale, "Unsupported spacing scale value.", out utility, out errorMessage))
            {
                return true;
            }

            return TryResolveMappedSingleProperty(token, "gap-", "gap", SpacingScale, "Unsupported spacing scale value.", out utility, out errorMessage);
        }

        private static bool TryResolveTracking(string token, out ResolvedUtility utility, out string errorMessage)
        {
            return TryResolveMappedSingleProperty(token, "tracking-", "letter-spacing", TrackingScale, "Unsupported tracking value.", out utility, out errorMessage);
        }

        private static bool TryResolveLeading(string token, out ResolvedUtility utility, out string errorMessage)
        {
            return TryResolveMappedSingleProperty(token, "leading-", "line-height", LeadingScale, "Unsupported leading value.", out utility, out errorMessage);
        }

        private static bool TryResolveColor(string token, out ResolvedUtility utility, out string errorMessage)
        {
            utility = null;
            errorMessage = null;

            if (token.StartsWith("bg-", StringComparison.Ordinal))
            {
                return TryResolvePaletteToken(token, "bg-", new[] { "background-color" }, out utility, out errorMessage);
            }

            if (token.StartsWith("text-", StringComparison.Ordinal))
            {
                return TryResolvePaletteToken(token, "text-", new[] { "color" }, out utility, out errorMessage);
            }

            if (token.StartsWith("border-", StringComparison.Ordinal))
            {
                return TryResolvePaletteToken(token, "border-", new[]
                {
                    "border-top-color",
                    "border-right-color",
                    "border-bottom-color",
                    "border-left-color"
                }, out utility, out errorMessage);
            }

            return false;
        }

        private static bool TryResolvePaletteToken(string token, string prefix, string[] properties, out ResolvedUtility utility, out string errorMessage)
        {
            utility = null;
            errorMessage = null;

            var key = token.Substring(prefix.Length);
            string value;
            if (!Colors.TryGetValue(key, out value))
            {
                errorMessage = "Unsupported color token.";
                return true;
            }

            utility = CreateBox(token, properties, value);
            return true;
        }

        private static bool TryResolveScaleBox(
            string token,
            IEnumerable<KeyValuePair<string, string[]>> definitions,
            IDictionary<string, string> scale,
            string errorMessage,
            out ResolvedUtility utility,
            out string resolveError)
        {
            utility = null;
            resolveError = null;

            foreach (var pair in definitions)
            {
                if (!token.StartsWith(pair.Key, StringComparison.Ordinal))
                {
                    continue;
                }

                var key = token.Substring(pair.Key.Length);
                string value;
                if (!scale.TryGetValue(key, out value))
                {
                    resolveError = errorMessage;
                    return true;
                }

                utility = CreateBox(token, pair.Value, value);
                return true;
            }

            return false;
        }

        private static bool TryResolveMappedSingleProperty(
            string token,
            string prefix,
            string propertyName,
            IDictionary<string, string> valueMap,
            string invalidMessage,
            out ResolvedUtility utility,
            out string errorMessage)
        {
            utility = null;
            errorMessage = null;
            if (!token.StartsWith(prefix, StringComparison.Ordinal))
            {
                return false;
            }

            return TryResolveMappedSinglePropertyCore(token.Substring(prefix.Length), token, propertyName, valueMap, invalidMessage, out utility, out errorMessage);
        }

        private static bool TryResolveMappedSinglePropertyCore(
            string key,
            string token,
            string propertyName,
            IDictionary<string, string> valueMap,
            string invalidMessage,
            out ResolvedUtility utility,
            out string errorMessage)
        {
            utility = null;
            errorMessage = null;

            string value;
            if (!valueMap.TryGetValue(key, out value))
            {
                errorMessage = invalidMessage;
                return true;
            }

            utility = Create(token, new StyleDeclaration(propertyName, value));
            return true;
        }

        private static ResolvedUtility Create(string token, params StyleDeclaration[] declarations)
        {
            return new ResolvedUtility(token, declarations);
        }

        private static ResolvedUtility Create(string token, IList<StyleDeclaration> declarations)
        {
            return new ResolvedUtility(token, declarations);
        }

        private static ResolvedUtility CreateBox(string token, IEnumerable<string> properties, string value)
        {
            var declarations = new List<StyleDeclaration>();
            foreach (var property in properties)
            {
                declarations.Add(new StyleDeclaration(property, value));
            }

            return new ResolvedUtility(token, declarations);
        }
    }
}
