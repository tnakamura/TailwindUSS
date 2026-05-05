using System;
using System.Collections.Generic;

namespace TailwindUSS.Editor
{
    internal sealed class UtilityResolver
    {
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
            { "lg", "18px" }
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

        public ResolveStatus TryResolve(string token, out ResolvedUtility utility, out string errorMessage)
        {
            utility = null;
            errorMessage = null;

            switch (token)
            {
                case "flex":
                    utility = Create(token, new StyleDeclaration("display", "flex"));
                    return ResolveStatus.Supported;
                case "hidden":
                    utility = Create(token, new StyleDeclaration("display", "none"));
                    return ResolveStatus.Supported;
                case "flex-row":
                    utility = Create(token, new StyleDeclaration("flex-direction", "row"));
                    return ResolveStatus.Supported;
                case "flex-col":
                    utility = Create(token, new StyleDeclaration("flex-direction", "column"));
                    return ResolveStatus.Supported;
                case "grow":
                    utility = Create(token, new StyleDeclaration("flex-grow", "1"));
                    return ResolveStatus.Supported;
                case "shrink":
                    utility = Create(token, new StyleDeclaration("flex-shrink", "1"));
                    return ResolveStatus.Supported;
                case "items-start":
                    utility = Create(token, new StyleDeclaration("align-items", "flex-start"));
                    return ResolveStatus.Supported;
                case "items-center":
                    utility = Create(token, new StyleDeclaration("align-items", "center"));
                    return ResolveStatus.Supported;
                case "items-end":
                    utility = Create(token, new StyleDeclaration("align-items", "flex-end"));
                    return ResolveStatus.Supported;
                case "justify-start":
                    utility = Create(token, new StyleDeclaration("justify-content", "flex-start"));
                    return ResolveStatus.Supported;
                case "justify-center":
                    utility = Create(token, new StyleDeclaration("justify-content", "center"));
                    return ResolveStatus.Supported;
                case "justify-end":
                    utility = Create(token, new StyleDeclaration("justify-content", "flex-end"));
                    return ResolveStatus.Supported;
                case "justify-between":
                    utility = Create(token, new StyleDeclaration("justify-content", "space-between"));
                    return ResolveStatus.Supported;
                case "font-normal":
                    utility = Create(token, new StyleDeclaration("-unity-font-style", "normal"));
                    return ResolveStatus.Supported;
                case "font-bold":
                    utility = Create(token, new StyleDeclaration("-unity-font-style", "bold"));
                    return ResolveStatus.Supported;
                case "border":
                    utility = CreateBox(token, new[] { "border-top-width", "border-right-width", "border-bottom-width", "border-left-width" }, "1px");
                    return ResolveStatus.Supported;
                case "border-0":
                    utility = CreateBox(token, new[] { "border-top-width", "border-right-width", "border-bottom-width", "border-left-width" }, "0px");
                    return ResolveStatus.Supported;
                case "border-2":
                    utility = CreateBox(token, new[] { "border-top-width", "border-right-width", "border-bottom-width", "border-left-width" }, "2px");
                    return ResolveStatus.Supported;
                case "rounded-none":
                    utility = CreateBox(token, RadiusProperties, "0px");
                    return ResolveStatus.Supported;
                case "rounded-sm":
                    utility = CreateBox(token, RadiusProperties, "2px");
                    return ResolveStatus.Supported;
                case "rounded":
                    utility = CreateBox(token, RadiusProperties, "4px");
                    return ResolveStatus.Supported;
                case "rounded-md":
                    utility = CreateBox(token, RadiusProperties, "6px");
                    return ResolveStatus.Supported;
                case "rounded-lg":
                    utility = CreateBox(token, RadiusProperties, "8px");
                    return ResolveStatus.Supported;
                case "rounded-full":
                    utility = CreateBox(token, RadiusProperties, "9999px");
                    return ResolveStatus.Supported;
            }

            if (TryResolveFontSize(token, out utility))
            {
                return ResolveStatus.Supported;
            }

            if (TryResolveSpacing(token, out utility, out errorMessage))
            {
                return errorMessage == null ? ResolveStatus.Supported : ResolveStatus.InvalidValue;
            }

            if (TryResolveSize(token, out utility, out errorMessage))
            {
                return errorMessage == null ? ResolveStatus.Supported : ResolveStatus.InvalidValue;
            }

            if (TryResolveColor(token, out utility, out errorMessage))
            {
                return errorMessage == null ? ResolveStatus.Supported : ResolveStatus.InvalidValue;
            }

            return ResolveStatus.Unsupported;
        }

        private static readonly string[] RadiusProperties =
        {
            "border-top-left-radius",
            "border-top-right-radius",
            "border-bottom-right-radius",
            "border-bottom-left-radius"
        };

        private static bool TryResolveFontSize(string token, out ResolvedUtility utility)
        {
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
            utility = null;
            errorMessage = null;

            foreach (var pair in SpacingDefinitions)
            {
                if (!token.StartsWith(pair.Key, StringComparison.Ordinal))
                {
                    continue;
                }

                var key = token.Substring(pair.Key.Length);
                string value;
                if (!SpacingScale.TryGetValue(key, out value))
                {
                    errorMessage = "Unsupported spacing scale value.";
                    return true;
                }

                utility = CreateBox(token, pair.Value, value);
                return true;
            }

            return false;
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

                var key = token.Substring(pair.Key.Length);
                string value;
                if (!SpacingScale.TryGetValue(key, out value))
                {
                    errorMessage = "Unsupported size scale value.";
                    return true;
                }

                utility = Create(token, new StyleDeclaration(pair.Value, value));
                return true;
            }

            return false;
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

        private static ResolvedUtility Create(string token, params StyleDeclaration[] declarations)
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
