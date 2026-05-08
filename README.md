# TailwindUSS

English | [日本語](README.ja.md)

TailwindUSS is a Unity Editor extension for UI Toolkit projects. It scans `class` attributes in UXML files, resolves a Tailwind-like utility subset, and generates a single USS file containing only the utilities used in the scanned files.

## Features

- Scans UXML files matched by configurable glob patterns
- Generates USS only for the utility tokens found in those files
- Validates unsupported utilities, unsupported variants, invalid values, and duplicate tokens
- Supports pseudo-class variants such as `hover:` and compound variants such as `hover:focus:`
- Can automatically insert a generated `<Style src="..." />` reference into scanned UXML files
- Lets you extend or override built-in color, spacing, and font-size scales
- Lets you define font and background-image aliases in configuration

## Requirements

- Unity 6.3 or newer
- For the current Unity USS compatibility matrix and remaining roadmap, see `docs/tailwind-uss-feature-matrix-ja.md` and `docs/tailwind-uss-implementation-plan-ja.md`.

## Installation

This repository contains a Unity Package Manager package in `src/TailwindUSS`.

Add it to your project's `Packages/manifest.json`:

```json
{
  "dependencies": {
    "com.tnakamura.tailwinduss": "https://github.com/tnakamura/TailwindUSS.git?path=/src/TailwindUSS"
  }
}
```

You can also add the same Git URL from the Unity Package Manager UI.

## Quick start

1. Install the package.
2. Create `tailwinduss.config.json` in the Unity project root, or use `Tools/TailwindUSS/Create Default Config`.
3. Add utility classes to UXML `class` attributes.
4. Run `Tools/TailwindUSS/Generate`.
5. Use the generated USS file from `outputUssPath`.

## Configuration

Create `tailwinduss.config.json` at the Unity project root.

```json
{
  "inputGlobs": ["Assets/**/*.uxml"],
  "outputUssPath": "Assets/Generated/TailwindUSS.generated.uss",
  "autoAttachGeneratedUss": false,
  "theme": {
    "colors": {
      "brand": "#112233"
    },
    "spacing": {
      "16": "64px"
    },
    "fontSizes": {
      "display": "42px"
    },
    "fonts": {
      "sans": "Fonts/Inter-Regular"
    },
    "backgroundImages": {
      "hero": "Images/Hero"
    }
  }
}
```

If the file does not exist, TailwindUSS uses in-memory defaults and shows a warning in the Unity Editor. Menu command: `Tools/TailwindUSS/Create Default Config`.

### Config fields

- `inputGlobs`: UXML files to scan. If empty, the default is effectively `Assets/**/*.uxml`.
- `outputUssPath`: Relative or absolute path of the generated USS file.
- `autoAttachGeneratedUss`: When `true`, TailwindUSS inserts a matching `Style` element at the top of each scanned UXML file if it is not already present.
- `theme.colors`: Extends or overrides built-in colors.
- `theme.spacing`: Extends or overrides built-in spacing and size tokens.
- `theme.fontSizes`: Extends or overrides built-in font sizes.
- `theme.fonts`: Maps `font-*` aliases to USS asset references. Plain strings are emitted as `resource("...")`; values already using `resource(...)` or `url(...)` are passed through.
- `theme.backgroundImages`: Maps `bg-*` aliases to `background-image` values using the same asset reference rules.

## Editor menu

- `Tools/TailwindUSS/Generate`
- `Tools/TailwindUSS/Validate`
- `Tools/TailwindUSS/Create Default Config`

## Diagnostics behavior

TailwindUSS reports the following in the Unity Editor:

- Warnings for unsupported utilities
- Warnings for unsupported variants
- Warnings for invalid scale or token values
- Warnings for duplicate tokens inside a single `class` attribute
- Errors for config loading failures
- Errors for UXML parse failures
- Errors for USS write failures
- Errors for automatic style-reference update failures

## Supported variants

- `hover:`
- `active:`
- `focus:`
- `disabled:`
- `checked:`
- `selected:`
- Compound variants such as `hover:focus:`

## Currently implemented utility subset

For the broader Unity USS compatibility matrix and the roadmap for still-unimplemented utilities, see:

- `docs/tailwind-uss-feature-matrix-ja.md`
- `docs/tailwind-uss-implementation-plan-ja.md`
- Those documents track the Unity 6.3+ filter support now implemented here and the remaining roadmap.

### Layout

- `flex`
- `hidden`
- `visible`
- `invisible`
- `flex-row`
- `flex-col`
- `flex-wrap`
- `flex-nowrap`
- `flex-wrap-reverse`
- `grow`
- `shrink`
- `overflow-hidden`
- `overflow-visible`
- `overflow-scroll`
- `relative`
- `absolute`
- `top-*`
- `right-*`
- `bottom-*`
- `left-*`
- `inset-*`
- `inset-x-*`
- `inset-y-*`
- `z-0`
- `z-10`
- `z-20`
- `z-30`
- `z-40`
- `z-50`
- `z-auto`
- `opacity-*`

### Alignment

- `items-start`
- `items-center`
- `items-end`
- `items-stretch`
- `justify-start`
- `justify-center`
- `justify-end`
- `justify-between`
- `justify-around`
- `justify-evenly`
- `self-auto`
- `self-start`
- `self-end`
- `self-center`
- `self-stretch`

### Spacing

- `p-*`
- `px-*`
- `py-*`
- `pt-*`
- `pr-*`
- `pb-*`
- `pl-*`
- `m-*`
- `mx-*`
- `my-*`
- `mt-*`
- `mr-*`
- `mb-*`
- `ml-*`
- `gap-*`
- `gap-x-*`
- `gap-y-*`

### Size

- `w-*`
- `h-*`
- `min-w-*`
- `min-h-*`
- `max-w-*`
- `max-h-*`
- `size-*`
- `w-auto`, `h-auto`, `w-1/2`, `h-1/2`, `w-full`, `h-full`
- `max-w-none`, `max-h-none`
- `basis-*`
- `basis-auto`, `basis-1/2`, `basis-full`
- `order-*`

### Color

- `bg-*`
- `bg-transparent`
- `bg-current`
- `text-*`
- `border-*`
- `border-t-*`
- `border-r-*`
- `border-b-*`
- `border-l-*`

### Background

- `bg-*` (configured background image aliases)
- `bg-none`
- `bg-cover`
- `bg-contain`
- `bg-center`
- `bg-top`
- `bg-bottom`
- `bg-left`
- `bg-right`
- `bg-repeat`
- `bg-no-repeat`
- `bg-repeat-x`
- `bg-repeat-y`

### Typography

- `text-xs`
- `text-sm`
- `text-base`
- `text-lg`
- `text-xl`
- `text-2xl`
- `text-3xl`
- `text-4xl`
- `text-5xl`
- `text-6xl`
- `text-7xl`
- `text-8xl`
- `text-9xl`
- `font-thin`
- `font-extralight`
- `font-light`
- `font-normal`
- `font-medium`
- `font-semibold`
- `font-bold`
- `font-extrabold`
- `font-black`
- `font-*` (configured font aliases)
- `text-left`
- `text-center`
- `text-right`
- `text-justify`
- `italic`
- `not-italic`
- `whitespace-normal`
- `whitespace-nowrap`
- `uppercase`
- `lowercase`
- `capitalize`
- `normal-case`
- `tracking-tighter`
- `tracking-tight`
- `tracking-normal`
- `tracking-wide`
- `tracking-wider`
- `tracking-widest`
- `leading-3`
- `leading-4`
- `leading-5`
- `leading-6`
- `leading-7`
- `leading-8`
- `leading-9`
- `leading-10`
- `truncate`
- `text-ellipsis`
- `text-clip`
- `break-normal`
- `break-all`
- `underline`
- `line-through`
- `no-underline`

### Border

- `border`
- `border-0`
- `border-2`
- `border-4`
- `border-8`
- `border-solid`
- `border-t`
- `border-r`
- `border-b`
- `border-l`

### Radius

- `rounded-none`
- `rounded-sm`
- `rounded`
- `rounded-md`
- `rounded-lg`
- `rounded-full`
- `rounded-t`
- `rounded-r`
- `rounded-b`
- `rounded-l`
- `rounded-tl`
- `rounded-tr`
- `rounded-br`
- `rounded-bl`
- `rounded-t-*`
- `rounded-r-*`
- `rounded-b-*`
- `rounded-l-*`
- `rounded-tl-*`
- `rounded-tr-*`
- `rounded-br-*`
- `rounded-bl-*`

### Transform

- `scale-*`
- `rotate-*`
- `translate-x-*`
- `translate-y-*`
- `origin-*`

### Filters

- `blur-none`, `blur-sm`, `blur`, `blur-md`, `blur-lg`, `blur-xl`, `blur-2xl`, `blur-3xl`
- `grayscale`, `grayscale-0`
- `invert`, `invert-0`
- `sepia`, `sepia-0`
- `contrast-0`, `contrast-50`, `contrast-75`, `contrast-100`, `contrast-125`, `contrast-150`, `contrast-200`
- `hue-rotate-0`, `hue-rotate-15`, `hue-rotate-30`, `hue-rotate-60`, `hue-rotate-90`, `hue-rotate-180`
- Multiple filter utilities used on the same element are emitted as a single composed `filter` declaration. When the same filter family appears more than once, TailwindUSS warns and uses the last token.

### Transition

- `transition`
- `transition-colors`
- `transition-opacity`
- `transition-transform`
- `duration-*`
- `delay-*`
- `ease-linear`
- `ease-in`
- `ease-out`
- `ease-in-out`

### Interaction

- `cursor-default`
- `cursor-pointer`
- `cursor-text`
- `cursor-move`
- `cursor-not-allowed`

## Token scales

`theme.colors`, `theme.spacing`, and `theme.fontSizes` can extend or override the built-in values below.

### Spacing and size scale

| token | value |
| --- | --- |
| 0 | 0px |
| 1 | 4px |
| 2 | 8px |
| 3 | 12px |
| 4 | 16px |
| 5 | 20px |
| 6 | 24px |
| 8 | 32px |
| 10 | 40px |
| 12 | 48px |

### Colors

- `white`
- `black`
- `gray-*`
- `slate-*`
- `zinc-*`
- `neutral-*`
- `stone-*`
- `blue-500`
- `red-500`
- `green-500`
- `yellow-500`
- `emerald-*`
- `sky-*`
- `indigo-*`
- `pink-*`

Supported palette steps for `gray`, `slate`, `zinc`, `neutral`, `stone`, `emerald`, `sky`, `indigo`, and `pink` are `100`, `300`, `500`, `700`, and `900`.

### Font sizes

| token | value |
| --- | --- |
| xs | 12px |
| sm | 14px |
| base | 16px |
| lg | 18px |
| xl | 20px |
| 2xl | 24px |
| 3xl | 30px |
| 4xl | 36px |
| 5xl | 48px |
| 6xl | 60px |
| 7xl | 72px |
| 8xl | 96px |
| 9xl | 128px |

### Additional built-in scales

- `size-*`, `w-*`, `h-*`, `basis-*`: spacing scale plus `auto`, `1/2`, and `full`
- `min-w-*`, `min-h-*`: spacing scale plus `1/2` and `full`
- `max-w-*`, `max-h-*`: spacing scale plus `1/2`, `full`, and `none`
- `font-*` weights: `thin`, `extralight`, `light`, `normal`, `medium` -> `font-weight: normal`; `semibold`, `bold`, `extrabold`, `black` -> `font-weight: bold`
- `text-decoration`: `underline`, `line-through`, `no-underline`
- `opacity-*`: `0`, `5`, `10`, `20`, `25`, `30`, `40`, `50`, `60`, `70`, `75`, `80`, `90`, `95`, `100`
- `z-*`: `0`, `10`, `20`, `30`, `40`, `50`, `auto`
- `order-*`: `0` through `12`
- `tracking-*`: `tighter`, `tight`, `normal`, `wide`, `wider`, `widest`
- `leading-*`: `3`, `4`, `5`, `6`, `7`, `8`, `9`, `10`
- `border-*` widths: `0`, `2`, `4`, `8` plus the fixed `border` shorthand for `1px`
- Radius values: `none`, `sm`, default, `md`, `lg`, `full`
- `scale-*`: `0`, `50`, `75`, `90`, `95`, `100`, `105`, `110`, `125`, `150`
- `rotate-*`: `0`, `1`, `2`, `3`, `6`, `12`, `45`, `90`, `180`
- `translate-x-*`, `translate-y-*`: spacing scale plus `1/2` and `full`
- `origin-*`: `center`, `top`, `top-right`, `right`, `bottom-right`, `bottom`, `bottom-left`, `left`, `top-left`
- `duration-*`, `delay-*`: `75`, `100`, `150`, `200`, `300`, `500`, `700`, `1000`
- `blur-*`: `none`, `sm`, default, `md`, `lg`, `xl`, `2xl`, `3xl`
- `contrast-*`: `0`, `50`, `75`, `100`, `125`, `150`, `200`
- `hue-rotate-*`: `0`, `15`, `30`, `60`, `90`, `180`

## Example

UXML:

```xml
<ui:VisualElement class="flex flex-row items-center px-4 py-2 bg-blue-500 rounded">
    <ui:Label class="text-sm text-white" text="Play" />
</ui:VisualElement>
```

Generated USS:

```css
/* Generated by TailwindUSS. */
/* Do not edit manually. */

.bg-blue-500 {
    background-color: #3B82F6;
}

.flex {
    display: flex;
}

.flex-row {
    flex-direction: row;
}

.items-center {
    align-items: center;
}

.px-4 {
    padding-left: 16px;
    padding-right: 16px;
}

.py-2 {
    padding-top: 8px;
    padding-bottom: 8px;
}

.rounded {
    border-top-left-radius: 4px;
    border-top-right-radius: 4px;
    border-bottom-right-radius: 4px;
    border-bottom-left-radius: 4px;
}

.text-sm {
    font-size: 14px;
}

.text-white {
    color: #FFFFFF;
}
```

## Development

### Automated tests

Run tests from the repository root:

```bash
dotnet test test/TailwindUSS.Editor.Tests/TailwindUSS.Editor.Tests.csproj
dotnet test test/TailwindUSS.Editor.Unit/TailwindUSS.Editor.Unit.csproj
```

Collect coverage in Cobertura format:

```bash
dotnet test test/TailwindUSS.Editor.Tests/TailwindUSS.Editor.Tests.csproj --settings test/coverlet.runsettings --collect:"XPlat Code Coverage"
```

Coverage reports are written under `test/TailwindUSS.Editor.Tests/TestResults/`.

## Package structure

- `src/TailwindUSS/package.json`: Unity package manifest
- `src/TailwindUSS/Editor/TailwindUSS.Editor.asmdef`: editor-only assembly definition
- `src/TailwindUSS/Editor/*.cs`: config loading, scanning, parsing, resolving, emitting, validation, generation, and menu commands

## Documentation

Additional project documents are currently maintained in Japanese:

- `docs/tailwind-uss-feature-matrix-ja.md`: Tailwind CSS / Unity USS compatibility matrix and implemented status
- `docs/tailwind-uss-implementation-plan-ja.md`: implementation roadmap for utilities not yet supported
- `docs/mvp-spec-ja.md`: MVP specification
- `docs/implementation-approaches-ja.md`: design notes and implementation approaches

## License

MIT
