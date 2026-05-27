# TailwindUSS skill

Copy this file to `.github/SKILL.md` in your Unity project, or merge it into the skill file that your coding agent reads when working with TailwindUSS.

## Use this skill when

- The Unity project uses TailwindUSS through `tailwinduss.config.json` or a `Packages/manifest.json` dependency on the TailwindUSS package name `com.github-tnakamura.tailwinduss` / sample-local alias `com.tnakamura.tailwinduss`.
- You are editing UI Toolkit UXML, generated USS integration, or `tailwinduss.config.json`.

## TailwindUSS workflow knowledge

- Read `tailwinduss.config.json` before editing UI files. Respect `inputGlobs`, `outputUssPath`, `autoAttachGeneratedUss`, `autoGenerateOnUxmlSave`, and `theme`.
- Prefer TailwindUSS utility classes in UXML `class` attributes over hand-written USS when the requested style is supported.
- Treat the generated USS at `outputUssPath` as machine-generated output. Do not hand-edit it.
- After changing scanned UXML or `tailwinduss.config.json`, regenerate styles with `Tools/TailwindUSS/Generate`.
- Review warnings from `Tools/TailwindUSS/Validate` after UI changes.

## Supported variants

- `hover:`
- `active:`
- `focus:`
- `disabled:`
- `checked:`
- `selected:`
- Compound variants such as `hover:focus:`

## Supported utility families

- Layout: `flex`, `hidden`, `visible`, `invisible`, `flex-row`, `flex-col`, `flex-wrap`, `flex-nowrap`, `flex-wrap-reverse`, `grow`, `shrink`, `overflow-*`, `relative`, `absolute`, `top-*`, `right-*`, `bottom-*`, `left-*`, `inset-*`, `inset-x-*`, `inset-y-*`, `z-*`, `opacity-*`
- Alignment: `items-*`, `justify-*`, `self-*`
- Spacing: `p-*`, `px-*`, `py-*`, `pt-*`, `pr-*`, `pb-*`, `pl-*`, `m-*`, `mx-*`, `my-*`, `mt-*`, `mr-*`, `mb-*`, `ml-*`
- Size and order: `w-*`, `h-*`, `min-w-*`, `min-h-*`, `max-w-*`, `max-h-*`, `size-*`, `basis-*`, `order-*`
- Colors and backgrounds: `bg-*`, `bg-transparent`, `bg-current`, `bg-none`, `bg-cover`, `bg-contain`, `bg-center`, `bg-top`, `bg-bottom`, `bg-left`, `bg-right`, `bg-repeat`, `bg-no-repeat`, `bg-repeat-x`, `bg-repeat-y`, `text-*`, `border-*`, `border-t-*`, `border-r-*`, `border-b-*`, `border-l-*`
- Typography: `text-xs` through `text-9xl`, `font-thin` through `font-black`, configured `font-*` aliases, `text-left`, `text-center`, `text-right`, `text-justify`, `italic`, `not-italic`, `whitespace-normal`, `whitespace-nowrap`, `tracking-*`, `truncate`, `text-ellipsis`, `text-clip`, `break-normal`, `break-all`, `underline`, `line-through`, `no-underline`
- Border and radius: `border`, `border-0`, `border-2`, `border-4`, `border-8`, `border-solid`, `border-t`, `border-r`, `border-b`, `border-l`, `rounded-none`, `rounded-sm`, `rounded`, `rounded-md`, `rounded-lg`, `rounded-full`, directional `rounded-*`
- Transform and effects: `scale-*`, `rotate-*`, `translate-x-*`, `translate-y-*`, `origin-*`, `blur-*`, `grayscale`, `grayscale-0`, `invert`, `invert-0`, `sepia`, `sepia-0`, `contrast-*`, `hue-rotate-*`
- Transition and interaction: `transition`, `transition-colors`, `transition-opacity`, `transition-transform`, `duration-*`, `delay-*`, `ease-linear`, `ease-in`, `ease-out`, `ease-in-out`, `cursor-default`, `cursor-pointer`, `cursor-text`, `cursor-move`, `cursor-not-allowed`

## Built-in token knowledge

- Spacing and shared size tokens: `0`, `1`, `2`, `3`, `4`, `5`, `6`, `8`, `10`, `12`
- Size aliases additionally support `auto`, `1/2`, `full`, and `none` where documented in TailwindUSS
- Font-size tokens: `xs`, `sm`, `base`, `lg`, `xl`, `2xl`, `3xl`, `4xl`, `5xl`, `6xl`, `7xl`, `8xl`, `9xl`
- Built-in colors: `white`, `black`, `gray-*`, `slate-*`, `zinc-*`, `neutral-*`, `stone-*`, `blue-500`, `red-500`, `green-500`, `yellow-500`, `emerald-*`, `sky-*`, `indigo-*`, `pink-*`
- Supported palette steps for `gray`, `slate`, `zinc`, `neutral`, `stone`, `emerald`, `sky`, `indigo`, and `pink`: `100`, `300`, `500`, `700`, `900`
- Theme aliases can extend or override `theme.colors`, `theme.spacing`, `theme.fontSizes`, `theme.fonts`, and `theme.backgroundImages`

## Important limitations

- Do not use `gap-*`, `row-gap-*`, or `column-gap-*`; Unity USS does not support them. Use margin utilities on child elements instead.
- Do not use text-case utilities such as `uppercase`; Unity USS has no `text-transform`.
- Do not use leading / `line-height` utilities; Unity USS cannot reproduce them.
- Unity cursor keywords differ from CSS: `cursor-default` maps to `arrow`, `cursor-pointer` to `link`, and `cursor-move` to `move-arrow`.
- If a requested style is unsupported by TailwindUSS or Unity USS, fall back to plain USS or C# instead of inventing unsupported utility classes.

## References

- `README.md`: supported utilities, scales, and examples
- `docs/tailwind-uss-feature-matrix-ja.md`: wider Unity USS compatibility notes
- `docs/tailwind-uss-implementation-plan-ja.md`: roadmap and remaining gaps
