# TailwindUSS sample project instructions

- This sample project uses TailwindUSS for UI Toolkit styling.
- Read `tailwinduss.config.json` before editing UI files. It scans `Assets/UI/**/*.uxml`, writes `Assets/Generated/TailwindUSS.generated.uss`, and auto-attaches the generated USS.
- Prefer editing utility classes in `Assets/UI/*.uxml` over hand-editing `Assets/Generated/TailwindUSS.generated.uss`.
- Keep `Assets/Generated/TailwindUSS.generated.uss` generated. After changing scanned UXML or config, regenerate it with `Tools/TailwindUSS/Generate`.
- Use the configured theme alias `bg-brand` for the sample brand color when it matches the design intent.
- Avoid `gap-*`; use margin utilities on child elements instead.
- If a style is unsupported by TailwindUSS or Unity USS, use plain USS or C# rather than introducing unsupported utility tokens.
