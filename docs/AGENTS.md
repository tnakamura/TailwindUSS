# TailwindUSS agent instructions

Copy this file to your Unity project's `.github/AGENTS.md`, or merge it into the agent instruction file that your coding agent reads. Pair it with the TailwindUSS `SKILL.md` file so the agent also has a compact list of supported features and utility classes.

## Detect TailwindUSS

- Treat the project as TailwindUSS-enabled when `tailwinduss.config.json` exists or `Packages/manifest.json` includes `com.github-tnakamura.tailwinduss` / `com.tnakamura.tailwinduss`.
- Read `tailwinduss.config.json` before editing UXML or USS. Respect `inputGlobs`, `outputUssPath`, `autoAttachGeneratedUss`, `autoGenerateOnUxmlSave`, and `theme`.
- When the project includes the TailwindUSS skill file, whether as `.github/SKILL.md` or merged into another skill file, read it before proposing new utility classes so you stay within the supported TailwindUSS subset.

## Preferred workflow

- Prefer TailwindUSS utility classes in UXML `class` attributes for UI Toolkit styling.
- Use aliases from `theme.colors`, `theme.spacing`, `theme.fontSizes`, `theme.fonts`, and `theme.backgroundImages` before inventing new literal values.
- Keep the generated USS at `outputUssPath` machine-generated. Do not hand-edit it; change UXML classes or `tailwinduss.config.json` instead.
- If `autoAttachGeneratedUss` is `false`, make sure each scanned UXML that needs the styles references the generated USS with a matching `<Style src="..." />`.
- After changing scanned UXML or `tailwinduss.config.json`, regenerate TailwindUSS output with `Tools/TailwindUSS/Generate`. If you cannot run Unity Editor yourself, tell the developer that regeneration is required.

## Guardrails

- Only use utilities and variants that TailwindUSS actually supports. Check the local TailwindUSS documentation when unsure.
- Prefer margin-based spacing instead of `gap-*`, because Unity UI Toolkit USS does not support gap utilities.
- If TailwindUSS or Unity USS cannot express a needed style, fall back to plain USS or C# instead of inventing unsupported utility classes.
- Preserve existing utility tokens unless the task intentionally changes the UI.
- When the generated USS is committed to source control, keep it in sync with the corresponding UXML and config changes.

## Validation

- Review warnings from `Tools/TailwindUSS/Validate` after UI changes.
- Confirm that committed generated USS changes match the edited UXML and config.
- Call out any unsupported utilities or required Unity-side regeneration in your final handoff.
