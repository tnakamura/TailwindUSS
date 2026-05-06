using System.Linq;

namespace TailwindUSS.Editor.Tests
{
    public sealed class UtilityResolverTests
    {
        [TestCase("flex", "display", "flex")]
        [TestCase("hidden", "display", "none")]
        [TestCase("flex-row", "flex-direction", "row")]
        [TestCase("flex-col", "flex-direction", "column")]
        [TestCase("grow", "flex-grow", "1")]
        [TestCase("shrink", "flex-shrink", "1")]
        [TestCase("items-start", "align-items", "flex-start")]
        [TestCase("items-center", "align-items", "center")]
        [TestCase("items-end", "align-items", "flex-end")]
        [TestCase("items-stretch", "align-items", "stretch")]
        [TestCase("justify-start", "justify-content", "flex-start")]
        [TestCase("justify-center", "justify-content", "center")]
        [TestCase("justify-end", "justify-content", "flex-end")]
        [TestCase("justify-between", "justify-content", "space-between")]
        [TestCase("justify-around", "justify-content", "space-around")]
        [TestCase("justify-evenly", "justify-content", "space-evenly")]
        [TestCase("font-normal", "-unity-font-style", "normal")]
        [TestCase("font-bold", "-unity-font-style", "bold")]
        [TestCase("italic", "-unity-font-style", "italic")]
        [TestCase("not-italic", "-unity-font-style", "normal")]
        [TestCase("text-sm", "font-size", "14px")]
        [TestCase("text-xl", "font-size", "20px")]
        [TestCase("text-3xl", "font-size", "30px")]
        [TestCase("text-left", "-unity-text-align", "middle-left")]
        [TestCase("text-center", "-unity-text-align", "middle-center")]
        [TestCase("text-right", "-unity-text-align", "middle-right")]
        [TestCase("text-justify", "-unity-text-align", "middle-left")]
        [TestCase("whitespace-normal", "white-space", "normal")]
        [TestCase("whitespace-nowrap", "white-space", "nowrap")]
        [TestCase("uppercase", "text-transform", "uppercase")]
        [TestCase("lowercase", "text-transform", "lowercase")]
        [TestCase("capitalize", "text-transform", "capitalize")]
        [TestCase("normal-case", "text-transform", "none")]
        [TestCase("tracking-wide", "letter-spacing", "0.025em")]
        [TestCase("leading-6", "line-height", "24px")]
        [TestCase("w-10", "width", "40px")]
        [TestCase("h-12", "height", "48px")]
        [TestCase("min-w-1", "min-width", "4px")]
        [TestCase("min-h-2", "min-height", "8px")]
        [TestCase("max-w-3", "max-width", "12px")]
        [TestCase("max-h-4", "max-height", "16px")]
        [TestCase("basis-4", "flex-basis", "16px")]
        [TestCase("order-4", "order", "4")]
        [TestCase("gap-4", "gap", "16px")]
        [TestCase("gap-x-2", "column-gap", "8px")]
        [TestCase("gap-y-3", "row-gap", "12px")]
        [TestCase("overflow-hidden", "overflow", "hidden")]
        [TestCase("overflow-visible", "overflow", "visible")]
        [TestCase("overflow-scroll", "overflow", "scroll")]
        [TestCase("relative", "position", "relative")]
        [TestCase("absolute", "position", "absolute")]
        [TestCase("z-10", "z-index", "10")]
        [TestCase("z-auto", "z-index", "auto")]
        [TestCase("opacity-50", "opacity", "0.5")]
        [TestCase("self-auto", "align-self", "auto")]
        [TestCase("self-start", "align-self", "flex-start")]
        [TestCase("self-end", "align-self", "flex-end")]
        [TestCase("self-center", "align-self", "center")]
        [TestCase("self-stretch", "align-self", "stretch")]
        [TestCase("flex-wrap", "flex-wrap", "wrap")]
        [TestCase("flex-nowrap", "flex-wrap", "nowrap")]
        [TestCase("flex-wrap-reverse", "flex-wrap", "wrap-reverse")]
        public void TryResolve_ResolvesSingleDeclarationUtilities(string token, string propertyName, string value)
        {
            var resolver = new UtilityResolver();

            var status = resolver.TryResolve(token, out var utility, out var errorMessage);

            Assert.That(status, Is.EqualTo(ResolveStatus.Supported));
            Assert.That(errorMessage, Is.Null);
            Assert.That(utility, Is.Not.Null);
            Assert.That(utility.Token, Is.EqualTo(token));
            Assert.That(utility.SelectorSuffix, Is.EqualTo(string.Empty));
            Assert.That(utility.Declarations.Select(declaration => declaration.PropertyName), Is.EqualTo(new[] { propertyName }));
            Assert.That(utility.Declarations.Select(declaration => declaration.Value), Is.EqualTo(new[] { value }));
        }

        [TestCase("px-4", new[] { "padding-left", "padding-right" }, "16px")]
        [TestCase("py-2", new[] { "padding-top", "padding-bottom" }, "8px")]
        [TestCase("pt-3", new[] { "padding-top" }, "12px")]
        [TestCase("pr-5", new[] { "padding-right" }, "20px")]
        [TestCase("pb-6", new[] { "padding-bottom" }, "24px")]
        [TestCase("pl-8", new[] { "padding-left" }, "32px")]
        [TestCase("p-10", new[] { "padding-top", "padding-right", "padding-bottom", "padding-left" }, "40px")]
        [TestCase("mx-12", new[] { "margin-left", "margin-right" }, "48px")]
        [TestCase("my-1", new[] { "margin-top", "margin-bottom" }, "4px")]
        [TestCase("mt-1", new[] { "margin-top" }, "4px")]
        [TestCase("mr-2", new[] { "margin-right" }, "8px")]
        [TestCase("mb-3", new[] { "margin-bottom" }, "12px")]
        [TestCase("ml-4", new[] { "margin-left" }, "16px")]
        [TestCase("m-0", new[] { "margin-top", "margin-right", "margin-bottom", "margin-left" }, "0px")]
        [TestCase("top-0", new[] { "top" }, "0px")]
        [TestCase("right-1", new[] { "right" }, "4px")]
        [TestCase("bottom-2", new[] { "bottom" }, "8px")]
        [TestCase("left-3", new[] { "left" }, "12px")]
        [TestCase("inset-4", new[] { "top", "right", "bottom", "left" }, "16px")]
        [TestCase("inset-x-5", new[] { "left", "right" }, "20px")]
        [TestCase("inset-y-6", new[] { "top", "bottom" }, "24px")]
        [TestCase("bg-blue-500", new[] { "background-color" }, "#3B82F6")]
        [TestCase("text-white", new[] { "color" }, "#FFFFFF")]
        [TestCase("border-red-500", new[] { "border-top-color", "border-right-color", "border-bottom-color", "border-left-color" }, "#EF4444")]
        [TestCase("border", new[] { "border-top-width", "border-right-width", "border-bottom-width", "border-left-width" }, "1px")]
        [TestCase("border-0", new[] { "border-top-width", "border-right-width", "border-bottom-width", "border-left-width" }, "0px")]
        [TestCase("border-2", new[] { "border-top-width", "border-right-width", "border-bottom-width", "border-left-width" }, "2px")]
        [TestCase("rounded-none", new[] { "border-top-left-radius", "border-top-right-radius", "border-bottom-right-radius", "border-bottom-left-radius" }, "0px")]
        [TestCase("rounded-sm", new[] { "border-top-left-radius", "border-top-right-radius", "border-bottom-right-radius", "border-bottom-left-radius" }, "2px")]
        [TestCase("rounded", new[] { "border-top-left-radius", "border-top-right-radius", "border-bottom-right-radius", "border-bottom-left-radius" }, "4px")]
        [TestCase("rounded-md", new[] { "border-top-left-radius", "border-top-right-radius", "border-bottom-right-radius", "border-bottom-left-radius" }, "6px")]
        [TestCase("rounded-lg", new[] { "border-top-left-radius", "border-top-right-radius", "border-bottom-right-radius", "border-bottom-left-radius" }, "8px")]
        [TestCase("rounded-full", new[] { "border-top-left-radius", "border-top-right-radius", "border-bottom-right-radius", "border-bottom-left-radius" }, "9999px")]
        public void TryResolve_ResolvesMultiDeclarationUtilities(string token, string[] properties, string value)
        {
            var resolver = new UtilityResolver();

            var status = resolver.TryResolve(token, out var utility, out var errorMessage);

            Assert.That(status, Is.EqualTo(ResolveStatus.Supported));
            Assert.That(errorMessage, Is.Null);
            Assert.That(utility, Is.Not.Null);
            Assert.That(utility.Declarations.Select(declaration => declaration.PropertyName), Is.EqualTo(properties));
            Assert.That(utility.Declarations.Select(declaration => declaration.Value), Is.All.EqualTo(value));
        }

        [TestCase("truncate", new[] { "overflow", "text-overflow", "white-space" }, new[] { "hidden", "ellipsis", "nowrap" })]
        public void TryResolve_ResolvesMultiDeclarationUtilitiesWithDistinctValues(string token, string[] properties, string[] values)
        {
            var resolver = new UtilityResolver();

            var status = resolver.TryResolve(token, out var utility, out var errorMessage);

            Assert.That(status, Is.EqualTo(ResolveStatus.Supported));
            Assert.That(errorMessage, Is.Null);
            Assert.That(utility, Is.Not.Null);
            Assert.That(utility.Declarations.Select(declaration => declaration.PropertyName), Is.EqualTo(properties));
            Assert.That(utility.Declarations.Select(declaration => declaration.Value), Is.EqualTo(values));
        }

        [TestCase("p-7", "Unsupported spacing scale value.")]
        [TestCase("w-7", "Unsupported size scale value.")]
        [TestCase("top-7", "Unsupported inset scale value.")]
        [TestCase("z-15", "Unsupported z-index value.")]
        [TestCase("opacity-73", "Unsupported opacity value.")]
        [TestCase("basis-7", "Unsupported size scale value.")]
        [TestCase("order-13", "Unsupported order value.")]
        [TestCase("gap-7", "Unsupported spacing scale value.")]
        [TestCase("tracking-super", "Unsupported tracking value.")]
        [TestCase("leading-11", "Unsupported leading value.")]
        [TestCase("bg-purple-500", "Unsupported color token.")]
        [TestCase("text-purple-500", "Unsupported color token.")]
        [TestCase("border-purple-500", "Unsupported color token.")]
        public void TryResolve_ReturnsInvalidValueForKnownPrefixesWithUnknownValues(string token, string expectedMessage)
        {
            var resolver = new UtilityResolver();

            var status = resolver.TryResolve(token, out var utility, out var errorMessage);

            Assert.That(status, Is.EqualTo(ResolveStatus.InvalidValue));
            Assert.That(utility, Is.Null);
            Assert.That(errorMessage, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void TryResolve_ReturnsUnsupportedForUnknownToken()
        {
            var resolver = new UtilityResolver();

            var status = resolver.TryResolve("grid", out var utility, out var errorMessage);

            Assert.That(status, Is.EqualTo(ResolveStatus.Unsupported));
            Assert.That(utility, Is.Null);
            Assert.That(errorMessage, Is.Null);
        }
    }
}
