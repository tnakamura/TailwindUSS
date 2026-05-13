using System;
using System.Collections.Generic;

namespace TailwindUSS.Editor
{
    /// <summary>
    /// Represents the utility resolver.
    /// </summary>
    internal sealed class UtilityResolver
    {
        private delegate bool UtilityHandler(string token, out ResolvedUtility utility, out string errorMessage);
        private readonly IDictionary<string, string> spacingScale;
        private readonly IDictionary<string, string> fontSizes;
        private readonly IDictionary<string, string> colors;
        private readonly IDictionary<string, string> fonts;
        private readonly IDictionary<string, string> backgroundImages;
        private readonly IDictionary<string, string> sizeScale;
        private readonly IDictionary<string, string> minSizeScale;
        private readonly IDictionary<string, string> maxSizeScale;
        private readonly IDictionary<string, string> basisScale;
        private readonly IDictionary<string, string> translateScale;

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

        private static readonly IDictionary<string, string> BlurScale = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            { "none", "0px" },
            { "sm", "4px" },
            { string.Empty, "8px" },
            { "md", "12px" },
            { "lg", "16px" },
            { "xl", "24px" },
            { "2xl", "40px" },
            { "3xl", "64px" }
        };

        private static readonly IDictionary<string, string> ContrastScale = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            { "0", "0%" },
            { "50", "50%" },
            { "75", "75%" },
            { "100", "100%" },
            { "125", "125%" },
            { "150", "150%" },
            { "200", "200%" }
        };

        private static readonly IDictionary<string, string> HueRotateScale = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            { "0", "0deg" },
            { "15", "15deg" },
            { "30", "30deg" },
            { "60", "60deg" },
            { "90", "90deg" },
            { "180", "180deg" }
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

        // Unity USS length values support px and %, so Tailwind's em-based tracking
        // scale is approximated against the default 16px base font size.
        private static readonly IDictionary<string, string> TrackingScale = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            { "tighter", "-0.8px" },
            { "tight", "-0.4px" },
            { "normal", "0" },
            { "wide", "0.4px" },
            { "wider", "0.8px" },
            { "widest", "1.6px" }
        };

        private static readonly IDictionary<string, string> FontWeightValues = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            { "thin", "normal" },
            { "extralight", "normal" },
            { "light", "normal" },
            { "normal", "normal" },
            { "medium", "normal" },
            { "semibold", "bold" },
            { "bold", "bold" },
            { "extrabold", "bold" },
            { "black", "bold" }
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

        private static readonly IDictionary<string, string> BorderWidthScale = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            { "0", "0px" },
            { "2", "2px" },
            { "4", "4px" },
            { "8", "8px" }
        };

        private static readonly IDictionary<string, string> RadiusScale = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            { "none", "0px" },
            { "sm", "2px" },
            { string.Empty, "4px" },
            { "md", "6px" },
            { "lg", "8px" },
            { "full", "9999px" }
        };

        private static readonly IDictionary<string, string> ScaleScale = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            { "0", "0" },
            { "50", "0.5" },
            { "75", "0.75" },
            { "90", "0.9" },
            { "95", "0.95" },
            { "100", "1" },
            { "105", "1.05" },
            { "110", "1.1" },
            { "125", "1.25" },
            { "150", "1.5" }
        };

        private static readonly IDictionary<string, string> RotateScale = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            { "0", "0deg" },
            { "1", "1deg" },
            { "2", "2deg" },
            { "3", "3deg" },
            { "6", "6deg" },
            { "12", "12deg" },
            { "45", "45deg" },
            { "90", "90deg" },
            { "180", "180deg" }
        };

        private static readonly IDictionary<string, string> TranslateScale = new Dictionary<string, string>(StringComparer.Ordinal)
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
            { "12", "48px" },
            { "1/2", "50%" },
            { "full", "100%" }
        };

        private static readonly IDictionary<string, string> TransformOriginValues = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            { "center", "50% 50%" },
            { "top", "50% 0%" },
            { "top-right", "100% 0%" },
            { "right", "100% 50%" },
            { "bottom-right", "100% 100%" },
            { "bottom", "50% 100%" },
            { "bottom-left", "0% 100%" },
            { "left", "0% 50%" },
            { "top-left", "0% 0%" }
        };

        private static readonly IDictionary<string, string> TransitionProperties = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            { string.Empty, "all" },
            { "colors", "background-color, border-top-color, border-right-color, border-bottom-color, border-left-color, color" },
            { "opacity", "opacity" },
            { "transform", "translate, rotate, scale" }
        };

        private static readonly IDictionary<string, string> TransitionDurationScale = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            { "75", "75ms" },
            { "100", "100ms" },
            { "150", "150ms" },
            { "200", "200ms" },
            { "300", "300ms" },
            { "500", "500ms" },
            { "700", "700ms" },
            { "1000", "1000ms" }
        };

        private static readonly IDictionary<string, string> EasingValues = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            { "linear", "linear" },
            { "in", "ease-in" },
            { "out", "ease-out" },
            { "in-out", "ease-in-out" }
        };

        private static readonly IDictionary<string, string> CursorValues = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            { "default", "default" },
            { "pointer", "pointer" },
            { "text", "text" },
            { "move", "move" },
            { "not-allowed", "not-allowed" }
        };

        private static readonly IDictionary<string, string> VariantSuffixes = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            { "hover", ":hover" },
            { "active", ":active" },
            { "focus", ":focus" },
            { "disabled", ":disabled" },
            { "checked", ":checked" },
            { "selected", ":selected" }
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
                { "italic", token => Create(token, new StyleDeclaration("-unity-font-style", "italic")) },
                { "not-italic", token => Create(token, new StyleDeclaration("-unity-font-style", "normal")) },
                { "underline", token => Create(token, new StyleDeclaration("text-decoration", "underline")) },
                { "line-through", token => Create(token, new StyleDeclaration("text-decoration", "line-through")) },
                { "no-underline", token => Create(token, new StyleDeclaration("text-decoration", "none")) },
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
                { "visible", token => Create(token, new StyleDeclaration("visibility", "visible")) },
                { "invisible", token => Create(token, new StyleDeclaration("visibility", "hidden")) },
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
                { "border-4", token => CreateBox(token, BorderWidthProperties, "4px") },
                { "border-8", token => CreateBox(token, BorderWidthProperties, "8px") },
                { "border-solid", token => CreateEmpty(token) },
                { "rounded-none", token => CreateBox(token, RadiusProperties, "0px") },
                { "rounded-sm", token => CreateBox(token, RadiusProperties, "2px") },
                { "rounded", token => CreateBox(token, RadiusProperties, "4px") },
                { "rounded-md", token => CreateBox(token, RadiusProperties, "6px") },
                { "rounded-lg", token => CreateBox(token, RadiusProperties, "8px") },
                { "rounded-full", token => CreateBox(token, RadiusProperties, "9999px") },
                { "bg-transparent", token => Create(token, new StyleDeclaration("background-color", "transparent")) },
                { "bg-current", token => Create(token, new StyleDeclaration("background-color", "currentColor")) },
                { "bg-cover", token => Create(token, new StyleDeclaration("background-size", "cover")) },
                { "bg-contain", token => Create(token, new StyleDeclaration("background-size", "contain")) },
                { "bg-center", token => Create(token, new StyleDeclaration("background-position", "center center")) },
                { "bg-top", token => Create(token, new StyleDeclaration("background-position", "center top")) },
                { "bg-bottom", token => Create(token, new StyleDeclaration("background-position", "center bottom")) },
                { "bg-left", token => Create(token, new StyleDeclaration("background-position", "left center")) },
                { "bg-right", token => Create(token, new StyleDeclaration("background-position", "right center")) },
                { "bg-repeat", token => Create(token, new StyleDeclaration("background-repeat", "repeat")) },
                { "bg-no-repeat", token => Create(token, new StyleDeclaration("background-repeat", "no-repeat")) },
                { "bg-repeat-x", token => Create(token, new StyleDeclaration("background-repeat", "repeat-x")) },
                { "bg-repeat-y", token => Create(token, new StyleDeclaration("background-repeat", "repeat-y")) },
                { "bg-none", token => Create(token, new StyleDeclaration("background-image", "none")) }
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

        private static readonly KeyValuePair<string, string>[] BorderSideWidthDefinitions =
        {
            new KeyValuePair<string, string>("border-t", "border-top-width"),
            new KeyValuePair<string, string>("border-r", "border-right-width"),
            new KeyValuePair<string, string>("border-b", "border-bottom-width"),
            new KeyValuePair<string, string>("border-l", "border-left-width")
        };

        private static readonly KeyValuePair<string, string>[] BorderSideColorDefinitions =
        {
            new KeyValuePair<string, string>("border-t", "border-top-color"),
            new KeyValuePair<string, string>("border-r", "border-right-color"),
            new KeyValuePair<string, string>("border-b", "border-bottom-color"),
            new KeyValuePair<string, string>("border-l", "border-left-color")
        };

        private static readonly KeyValuePair<string, string[]>[] RoundedDefinitions =
        {
            new KeyValuePair<string, string[]>("rounded-t-", new[] { "border-top-left-radius", "border-top-right-radius" }),
            new KeyValuePair<string, string[]>("rounded-r-", new[] { "border-top-right-radius", "border-bottom-right-radius" }),
            new KeyValuePair<string, string[]>("rounded-b-", new[] { "border-bottom-right-radius", "border-bottom-left-radius" }),
            new KeyValuePair<string, string[]>("rounded-l-", new[] { "border-top-left-radius", "border-bottom-left-radius" }),
            new KeyValuePair<string, string[]>("rounded-tl-", new[] { "border-top-left-radius" }),
            new KeyValuePair<string, string[]>("rounded-tr-", new[] { "border-top-right-radius" }),
            new KeyValuePair<string, string[]>("rounded-br-", new[] { "border-bottom-right-radius" }),
            new KeyValuePair<string, string[]>("rounded-bl-", new[] { "border-bottom-left-radius" })
        };

        private static readonly KeyValuePair<string, string[]>[] RoundedExactDefinitions =
        {
            new KeyValuePair<string, string[]>("rounded-t", new[] { "border-top-left-radius", "border-top-right-radius" }),
            new KeyValuePair<string, string[]>("rounded-r", new[] { "border-top-right-radius", "border-bottom-right-radius" }),
            new KeyValuePair<string, string[]>("rounded-b", new[] { "border-bottom-right-radius", "border-bottom-left-radius" }),
            new KeyValuePair<string, string[]>("rounded-l", new[] { "border-top-left-radius", "border-bottom-left-radius" }),
            new KeyValuePair<string, string[]>("rounded-tl", new[] { "border-top-left-radius" }),
            new KeyValuePair<string, string[]>("rounded-tr", new[] { "border-top-right-radius" }),
            new KeyValuePair<string, string[]>("rounded-br", new[] { "border-bottom-right-radius" }),
            new KeyValuePair<string, string[]>("rounded-bl", new[] { "border-bottom-left-radius" })
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="UtilityResolver"/> type.
        /// </summary>
        public UtilityResolver(TailwindUssTheme theme = null)
        {
            var mergedTheme = TailwindUssTheme.CreateMerged(theme);
            spacingScale = mergedTheme.spacing;
            fontSizes = mergedTheme.fontSizes;
            colors = mergedTheme.colors;
            fonts = mergedTheme.fonts;
            backgroundImages = mergedTheme.backgroundImages;
            sizeScale = CreateSizeScale(spacingScale);
            minSizeScale = CreateMinSizeScale(spacingScale);
            maxSizeScale = CreateMaxSizeScale(spacingScale);
            basisScale = CreateBasisScale(spacingScale);
            translateScale = CreateTranslateScale(spacingScale);
        }

        /// <summary>
        /// Attempts to resolve a utility token to its declarations.
        /// </summary>
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

            if (TryResolveFontSize(token, out utility, out errorMessage)
                || TryResolveFontWeight(token, out utility, out errorMessage)
                || TryResolveFontFamily(token, out utility, out errorMessage)
                || TryResolveSpacing(token, out utility, out errorMessage)
                || TryResolveInset(token, out utility, out errorMessage)
                || TryResolveOverflow(token, out utility, out errorMessage)
                || TryResolveZIndex(token, out utility, out errorMessage)
                || TryResolveOpacity(token, out utility, out errorMessage)
                || TryResolveFilter(token, out utility, out errorMessage)
                || TryResolveSize(token, out utility, out errorMessage)
                || TryResolveBasis(token, out utility, out errorMessage)
                || TryResolveOrder(token, out utility, out errorMessage)
                || TryResolveGap(token, out utility, out errorMessage)
                || TryResolveTracking(token, out utility, out errorMessage)
                || TryResolveLeading(token, out utility, out errorMessage)
                || TryResolveTransform(token, out utility, out errorMessage)
                || TryResolveTransformOrigin(token, out utility, out errorMessage)
                || TryResolveTransitionProperty(token, out utility, out errorMessage)
                || TryResolveTransitionDuration(token, out utility, out errorMessage)
                || TryResolveTransitionDelay(token, out utility, out errorMessage)
                || TryResolveTransitionTiming(token, out utility, out errorMessage)
                || TryResolveCursor(token, out utility, out errorMessage)
                || TryResolveBorder(token, out utility, out errorMessage)
                || TryResolveRounded(token, out utility, out errorMessage)
                || TryResolveBackground(token, out utility, out errorMessage)
                || TryResolveColor(token, out utility, out errorMessage))
            {
                return errorMessage == null ? ResolveStatus.Supported : ResolveStatus.InvalidValue;
            }

            return ResolveStatus.Unsupported;
        }

        /// <summary>
        /// Attempts to resolve a token occurrence, including its variant chain, to declarations.
        /// </summary>
        public ResolveStatus TryResolve(UxmlTokenOccurrence occurrence, out ResolvedUtility utility, out string errorMessage)
        {
            var status = TryResolve(occurrence.BaseToken, out utility, out errorMessage);
            if (status != ResolveStatus.Supported)
            {
                return status;
            }

            var selectorSuffix = utility.SelectorSuffix;
            foreach (var variant in occurrence.VariantChain)
            {
                string suffix;
                if (!VariantSuffixes.TryGetValue(variant, out suffix))
                {
                    utility = null;
                    errorMessage = string.Format("Unsupported variant '{0}'.", variant);
                    return ResolveStatus.UnsupportedVariant;
                }

                selectorSuffix += suffix;
            }

            utility = new ResolvedUtility(
                occurrence.OriginalToken,
                utility.Declarations,
                selectorSuffix,
                filterContribution: utility.FilterContribution);
            errorMessage = null;
            return ResolveStatus.Supported;
        }

        private bool TryResolveFontSize(string token, out ResolvedUtility utility, out string errorMessage)
        {
            errorMessage = null;
            utility = null;
            if (!token.StartsWith("text-", StringComparison.Ordinal))
            {
                return false;
            }

            var key = token.Substring("text-".Length);
            string value;
            if (!fontSizes.TryGetValue(key, out value))
            {
                return false;
            }

            utility = Create(token, new StyleDeclaration("font-size", value));
            return true;
        }

        private bool TryResolveFontFamily(string token, out ResolvedUtility utility, out string errorMessage)
        {
            utility = null;
            errorMessage = null;
            if (!token.StartsWith("font-", StringComparison.Ordinal))
            {
                return false;
            }

            var key = token.Substring("font-".Length);
            string value;
            if (!fonts.TryGetValue(key, out value))
            {
                errorMessage = "Unsupported font alias.";
                return true;
            }

            utility = Create(token, new StyleDeclaration("-unity-font", FormatAssetReference(value)));
            return true;
        }

        private static bool TryResolveFontWeight(string token, out ResolvedUtility utility, out string errorMessage)
        {
            utility = null;
            errorMessage = null;
            if (!token.StartsWith("font-", StringComparison.Ordinal))
            {
                return false;
            }

            var key = token.Substring("font-".Length);
            string value;
            if (!FontWeightValues.TryGetValue(key, out value))
            {
                return false;
            }

            utility = Create(token, new StyleDeclaration("font-weight", value));
            return true;
        }

        private bool TryResolveSpacing(string token, out ResolvedUtility utility, out string errorMessage)
        {
            return TryResolveScaleBox(token, SpacingDefinitions, spacingScale, "Unsupported spacing scale value.", out utility, out errorMessage);
        }

        private bool TryResolveInset(string token, out ResolvedUtility utility, out string errorMessage)
        {
            return TryResolveScaleBox(token, InsetDefinitions, spacingScale, "Unsupported inset scale value.", out utility, out errorMessage);
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

        private static bool TryResolveFilter(string token, out ResolvedUtility utility, out string errorMessage)
        {
            if (token == "blur")
            {
                utility = CreateFilter(token, "blur", string.Format("blur({0})", BlurScale[string.Empty]));
                errorMessage = null;
                return true;
            }

            if (TryResolveMappedFilter(token, "blur-", "blur", BlurScale, "Unsupported blur value.", "blur({0})", out utility, out errorMessage))
            {
                return true;
            }

            if (TryResolveFixedFilter(token, "grayscale", "grayscale", "grayscale(100%)", out utility, out errorMessage)
                || TryResolveFixedFilter(token, "grayscale-0", "grayscale", "grayscale(0%)", out utility, out errorMessage)
                || TryResolveInvalidFixedFilter(token, "grayscale-", "Unsupported grayscale value.", out utility, out errorMessage)
                || TryResolveFixedFilter(token, "invert", "invert", "invert(100%)", out utility, out errorMessage)
                || TryResolveFixedFilter(token, "invert-0", "invert", "invert(0%)", out utility, out errorMessage)
                || TryResolveInvalidFixedFilter(token, "invert-", "Unsupported invert value.", out utility, out errorMessage)
                || TryResolveFixedFilter(token, "sepia", "sepia", "sepia(100%)", out utility, out errorMessage)
                || TryResolveFixedFilter(token, "sepia-0", "sepia", "sepia(0%)", out utility, out errorMessage)
                || TryResolveInvalidFixedFilter(token, "sepia-", "Unsupported sepia value.", out utility, out errorMessage)
                || TryResolveMappedFilter(token, "contrast-", "contrast", ContrastScale, "Unsupported contrast value.", "contrast({0})", out utility, out errorMessage)
                || TryResolveMappedFilter(token, "hue-rotate-", "hue-rotate", HueRotateScale, "Unsupported hue-rotate value.", "hue-rotate({0})", out utility, out errorMessage))
            {
                return true;
            }

            utility = null;
            errorMessage = null;
            return false;
        }

        private bool TryResolveSize(string token, out ResolvedUtility utility, out string errorMessage)
        {
            if (TryResolveScaleBox(
                token,
                new[] { new KeyValuePair<string, string[]>("size-", new[] { "width", "height" }) },
                sizeScale,
                "Unsupported size scale value.",
                out utility,
                out errorMessage))
            {
                return true;
            }

            if (TryResolveMappedSingleProperty(token, "min-w-", "min-width", minSizeScale, "Unsupported size scale value.", out utility, out errorMessage)
                || TryResolveMappedSingleProperty(token, "min-h-", "min-height", minSizeScale, "Unsupported size scale value.", out utility, out errorMessage)
                || TryResolveMappedSingleProperty(token, "max-w-", "max-width", maxSizeScale, "Unsupported size scale value.", out utility, out errorMessage)
                || TryResolveMappedSingleProperty(token, "max-h-", "max-height", maxSizeScale, "Unsupported size scale value.", out utility, out errorMessage)
                || TryResolveMappedSingleProperty(token, "w-", "width", sizeScale, "Unsupported size scale value.", out utility, out errorMessage)
                || TryResolveMappedSingleProperty(token, "h-", "height", sizeScale, "Unsupported size scale value.", out utility, out errorMessage))
            {
                return true;
            }

            utility = null;
            errorMessage = null;
            return false;
        }

        private bool TryResolveBasis(string token, out ResolvedUtility utility, out string errorMessage)
        {
            return TryResolveMappedSingleProperty(token, "basis-", "flex-basis", basisScale, "Unsupported size scale value.", out utility, out errorMessage);
        }

        private static bool TryResolveOrder(string token, out ResolvedUtility utility, out string errorMessage)
        {
            return TryResolveMappedSingleProperty(token, "order-", "order", OrderScale, "Unsupported order value.", out utility, out errorMessage);
        }

        private bool TryResolveGap(string token, out ResolvedUtility utility, out string errorMessage)
        {
            if (TryResolveMappedSingleProperty(token, "gap-x-", "column-gap", spacingScale, "Unsupported spacing scale value.", out utility, out errorMessage))
            {
                return true;
            }

            if (TryResolveMappedSingleProperty(token, "gap-y-", "row-gap", spacingScale, "Unsupported spacing scale value.", out utility, out errorMessage))
            {
                return true;
            }

            return TryResolveMappedSingleProperty(token, "gap-", "gap", spacingScale, "Unsupported spacing scale value.", out utility, out errorMessage);
        }

        private static bool TryResolveTracking(string token, out ResolvedUtility utility, out string errorMessage)
        {
            return TryResolveMappedSingleProperty(token, "tracking-", "letter-spacing", TrackingScale, "Unsupported tracking value.", out utility, out errorMessage);
        }

        private static bool TryResolveLeading(string token, out ResolvedUtility utility, out string errorMessage)
        {
            return TryResolveMappedSingleProperty(token, "leading-", "line-height", LeadingScale, "Unsupported leading value.", out utility, out errorMessage);
        }

        private bool TryResolveTransform(string token, out ResolvedUtility utility, out string errorMessage)
        {
            if (TryResolveMappedSingleProperty(token, "scale-", "scale", ScaleScale, "Unsupported scale value.", out utility, out errorMessage))
            {
                return true;
            }

            if (TryResolveMappedSingleProperty(token, "rotate-", "rotate", RotateScale, "Unsupported rotate value.", out utility, out errorMessage))
            {
                return true;
            }

            if (token.StartsWith("translate-x-", StringComparison.Ordinal))
            {
                return TryResolveTranslate(token, "translate-x-", true, out utility, out errorMessage);
            }

            if (token.StartsWith("translate-y-", StringComparison.Ordinal))
            {
                return TryResolveTranslate(token, "translate-y-", false, out utility, out errorMessage);
            }

            utility = null;
            errorMessage = null;
            return false;
        }

        private static bool TryResolveTransformOrigin(string token, out ResolvedUtility utility, out string errorMessage)
        {
            return TryResolveMappedSingleProperty(token, "origin-", "transform-origin", TransformOriginValues, "Unsupported transform origin value.", out utility, out errorMessage);
        }

        private static bool TryResolveTransitionProperty(string token, out ResolvedUtility utility, out string errorMessage)
        {
            errorMessage = null;
            utility = null;

            if (token == "transition")
            {
                utility = Create(token, new StyleDeclaration("transition-property", TransitionProperties[string.Empty]));
                return true;
            }

            if (!token.StartsWith("transition-", StringComparison.Ordinal))
            {
                return false;
            }

            var key = token.Substring("transition-".Length);
            string value;
            if (!TransitionProperties.TryGetValue(key, out value))
            {
                errorMessage = "Unsupported transition property value.";
                return true;
            }

            utility = Create(token, new StyleDeclaration("transition-property", value));
            return true;
        }

        private static bool TryResolveTransitionDuration(string token, out ResolvedUtility utility, out string errorMessage)
        {
            return TryResolveMappedSingleProperty(token, "duration-", "transition-duration", TransitionDurationScale, "Unsupported transition duration value.", out utility, out errorMessage);
        }

        private static bool TryResolveTransitionDelay(string token, out ResolvedUtility utility, out string errorMessage)
        {
            return TryResolveMappedSingleProperty(token, "delay-", "transition-delay", TransitionDurationScale, "Unsupported transition delay value.", out utility, out errorMessage);
        }

        private static bool TryResolveTransitionTiming(string token, out ResolvedUtility utility, out string errorMessage)
        {
            return TryResolveMappedSingleProperty(token, "ease-", "transition-timing-function", EasingValues, "Unsupported easing value.", out utility, out errorMessage);
        }

        private static bool TryResolveCursor(string token, out ResolvedUtility utility, out string errorMessage)
        {
            return TryResolveMappedSingleProperty(token, "cursor-", "cursor", CursorValues, "Unsupported cursor value.", out utility, out errorMessage);
        }

        private bool TryResolveBorder(string token, out ResolvedUtility utility, out string errorMessage)
        {
            utility = null;
            errorMessage = null;

            foreach (var side in BorderSideWidthDefinitions)
            {
                if (token == side.Key)
                {
                    utility = Create(token, new StyleDeclaration(side.Value, "1px"));
                    return true;
                }

                var prefix = side.Key + "-";
                if (!token.StartsWith(prefix, StringComparison.Ordinal))
                {
                    continue;
                }

                var key = token.Substring(prefix.Length);
                string value;
                if (BorderWidthScale.TryGetValue(key, out value))
                {
                    utility = Create(token, new StyleDeclaration(side.Value, value));
                    return true;
                }

                foreach (var colorSide in BorderSideColorDefinitions)
                {
                    if (colorSide.Key != side.Key)
                    {
                        continue;
                    }

                    if (colors.TryGetValue(key, out value))
                    {
                        utility = Create(token, new StyleDeclaration(colorSide.Value, value));
                        return true;
                    }
                }

                errorMessage = IsNumericToken(key) ? "Unsupported border width value." : "Unsupported color token.";
                return true;
            }

            if (!token.StartsWith("border-", StringComparison.Ordinal))
            {
                return false;
            }

            var globalKey = token.Substring("border-".Length);
            string globalValue;
            if (BorderWidthScale.TryGetValue(globalKey, out globalValue))
            {
                utility = CreateBox(token, BorderWidthProperties, globalValue);
                return true;
            }

            if (colors.TryGetValue(globalKey, out globalValue))
            {
                utility = CreateBox(token, new[]
                {
                    "border-top-color",
                    "border-right-color",
                    "border-bottom-color",
                    "border-left-color"
                }, globalValue);
                return true;
            }

            errorMessage = IsNumericToken(globalKey) ? "Unsupported border width value." : "Unsupported color token.";
            return true;
        }

        private static bool TryResolveRounded(string token, out ResolvedUtility utility, out string errorMessage)
        {
            utility = null;
            errorMessage = null;

            foreach (var pair in RoundedExactDefinitions)
            {
                if (token != pair.Key)
                {
                    continue;
                }

                utility = CreateBox(token, pair.Value, RadiusScale[string.Empty]);
                return true;
            }

            foreach (var pair in RoundedDefinitions)
            {
                if (!token.StartsWith(pair.Key, StringComparison.Ordinal))
                {
                    continue;
                }

                var key = token.Substring(pair.Key.Length);
                string value;
                if (!RadiusScale.TryGetValue(key, out value))
                {
                    errorMessage = "Unsupported radius value.";
                    return true;
                }

                utility = CreateBox(token, pair.Value, value);
                return true;
            }

            if (!token.StartsWith("rounded-", StringComparison.Ordinal))
            {
                return false;
            }

            errorMessage = "Unsupported radius value.";
            return true;
        }

        private bool TryResolveBackground(string token, out ResolvedUtility utility, out string errorMessage)
        {
            utility = null;
            errorMessage = null;

            if (!token.StartsWith("bg-", StringComparison.Ordinal))
            {
                return false;
            }

            var key = token.Substring("bg-".Length);
            string value;
            if (colors.TryGetValue(key, out value))
            {
                utility = Create(token, new StyleDeclaration("background-color", value));
                return true;
            }

            if (backgroundImages.TryGetValue(key, out value))
            {
                utility = Create(token, new StyleDeclaration("background-image", FormatAssetReference(value)));
                return true;
            }

            errorMessage = LooksLikeColorToken(key) || key == "transparent" || key == "current"
                ? "Unsupported color token."
                : "Unsupported background utility value.";
            return true;
        }

        private bool TryResolveColor(string token, out ResolvedUtility utility, out string errorMessage)
        {
            utility = null;
            errorMessage = null;

            if (token.StartsWith("text-", StringComparison.Ordinal))
            {
                return TryResolvePaletteToken(token, "text-", new[] { "color" }, out utility, out errorMessage);
            }

            return false;
        }

        private bool TryResolvePaletteToken(string token, string prefix, string[] properties, out ResolvedUtility utility, out string errorMessage)
        {
            utility = null;
            errorMessage = null;

            var key = token.Substring(prefix.Length);
            string value;
            if (!colors.TryGetValue(key, out value))
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

        private static bool TryResolveFixedFilter(
            string token,
            string exactToken,
            string family,
            string function,
            out ResolvedUtility utility,
            out string errorMessage)
        {
            if (token != exactToken)
            {
                utility = null;
                errorMessage = null;
                return false;
            }

            utility = CreateFilter(token, family, function);
            errorMessage = null;
            return true;
        }

        private static bool TryResolveInvalidFixedFilter(
            string token,
            string prefix,
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

            errorMessage = invalidMessage;
            return true;
        }

        private static bool TryResolveMappedFilter(
            string token,
            string prefix,
            string family,
            IDictionary<string, string> valueMap,
            string invalidMessage,
            string functionFormat,
            out ResolvedUtility utility,
            out string errorMessage)
        {
            utility = null;
            errorMessage = null;
            if (!token.StartsWith(prefix, StringComparison.Ordinal))
            {
                return false;
            }

            string value;
            if (!valueMap.TryGetValue(token.Substring(prefix.Length), out value))
            {
                errorMessage = invalidMessage;
                return true;
            }

            utility = CreateFilter(token, family, string.Format(functionFormat, value));
            return true;
        }

        private bool TryResolveTranslate(
            string token,
            string prefix,
            bool isX,
            out ResolvedUtility utility,
            out string errorMessage)
        {
            utility = null;
            errorMessage = null;

            var key = token.Substring(prefix.Length);
            string value;
            if (!translateScale.TryGetValue(key, out value))
            {
                errorMessage = "Unsupported translate value.";
                return true;
            }

            utility = Create(token, new StyleDeclaration("translate", isX ? string.Format("{0} 0", value) : string.Format("0 {0}", value)));
            return true;
        }

        private static ResolvedUtility Create(string token, params StyleDeclaration[] declarations)
        {
            return new ResolvedUtility(token, declarations);
        }

        private static ResolvedUtility CreateEmpty(string token)
        {
            return new ResolvedUtility(token, Array.Empty<StyleDeclaration>());
        }

        private static ResolvedUtility Create(string token, IList<StyleDeclaration> declarations)
        {
            return new ResolvedUtility(token, declarations);
        }

        private static ResolvedUtility CreateFilter(string token, string family, string function)
        {
            return new ResolvedUtility(
                token,
                new[] { new StyleDeclaration("filter", function) },
                filterContribution: new FilterContribution(family, function));
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

        private static IDictionary<string, string> CreateTranslateScale(IDictionary<string, string> spacing)
        {
            return CreateMergedScale(
                spacing,
                new KeyValuePair<string, string>("1/2", "50%"),
                new KeyValuePair<string, string>("full", "100%"));
        }

        private static IDictionary<string, string> CreateSizeScale(IDictionary<string, string> spacing)
        {
            return CreateMergedScale(
                spacing,
                new KeyValuePair<string, string>("1/2", "50%"),
                new KeyValuePair<string, string>("full", "100%"),
                new KeyValuePair<string, string>("auto", "auto"));
        }

        private static IDictionary<string, string> CreateMinSizeScale(IDictionary<string, string> spacing)
        {
            return CreateMergedScale(
                spacing,
                new KeyValuePair<string, string>("1/2", "50%"),
                new KeyValuePair<string, string>("full", "100%"));
        }

        private static IDictionary<string, string> CreateMaxSizeScale(IDictionary<string, string> spacing)
        {
            return CreateMergedScale(
                spacing,
                new KeyValuePair<string, string>("1/2", "50%"),
                new KeyValuePair<string, string>("full", "100%"),
                new KeyValuePair<string, string>("none", "none"));
        }

        private static IDictionary<string, string> CreateBasisScale(IDictionary<string, string> spacing)
        {
            return CreateMergedScale(
                spacing,
                new KeyValuePair<string, string>("1/2", "50%"),
                new KeyValuePair<string, string>("full", "100%"),
                new KeyValuePair<string, string>("auto", "auto"));
        }

        private static IDictionary<string, string> CreateMergedScale(
            IDictionary<string, string> source,
            params KeyValuePair<string, string>[] additions)
        {
            var merged = new Dictionary<string, string>(source, StringComparer.Ordinal);
            foreach (var addition in additions)
            {
                if (!merged.ContainsKey(addition.Key))
                {
                    merged[addition.Key] = addition.Value;
                }
            }

            return merged;
        }

        private static string FormatAssetReference(string value)
        {
            if (value.IndexOf('(') >= 0)
            {
                return value;
            }

            return string.Format("resource(\"{0}\")", EscapeString(value));
        }

        private static string EscapeString(string value)
        {
            return value.Replace("\\", "\\\\").Replace("\"", "\\\"");
        }

        private static bool IsNumericToken(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return false;
            }

            foreach (var character in token)
            {
                if (!char.IsDigit(character))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool LooksLikeColorToken(string token)
        {
            return token.Contains("-") || token == "white" || token == "black";
        }
    }
}
