# TailwindUSS sample project skill

- Use this skill when editing UI Toolkit files in this sample project.
- Read `tailwinduss.config.json` first. This sample scans `Assets/UI/**/*.uxml`, writes `Assets/Generated/TailwindUSS.generated.uss`, and auto-attaches the generated USS.
- Prefer TailwindUSS utility classes in `Assets/UI/*.uxml` over hand-editing `Assets/Generated/TailwindUSS.generated.uss`.
- Supported variants here are `hover:`, `active:`, `focus:`, `disabled:`, `checked:`, `selected:`, and compound variants such as `hover:focus:`.
- Prefer supported utility families from TailwindUSS: layout, alignment, spacing, size, color, background, typography, border, radius, transform, filters, transition, and cursor utilities.
- Use `bg-brand` when the design calls for the sample brand color from `theme.colors`.
- Avoid unsupported utilities such as `gap-*`, text-transform utilities, and leading utilities; use margin utilities, plain USS, or C# when needed.
- After changing scanned UXML or config, regenerate the checked-in USS with `Tools/TailwindUSS/Generate`.
- Review `README.md` in the repository root or `docs/SKILL.example.md` for the fuller supported utility list.
