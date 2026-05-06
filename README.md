# TailwindUSS

TailwindUSS is a Unity Editor extension that scans UI Toolkit UXML `class` attributes, resolves a Tailwind-like utility subset, and generates a single USS file for the current project.

## Features

- Scans project UXML files matched by `tailwinduss.config.json`
- Generates only the utility classes actually used in those files
- Validates unsupported tokens and invalid values from the Unity Editor
- Creates a default JSON config from the Unity Editor
- Optionally adds the generated USS reference to scanned UXML files

## Installation

Add this repository as a Unity Package Manager dependency.

## Automated tests

Run the unit tests from the repository root:

```bash
dotnet test Tests/TailwindUSS.Editor.Tests/TailwindUSS.Editor.Tests.csproj
```

Collect coverage in Cobertura format:

```bash
dotnet test Tests/TailwindUSS.Editor.Tests/TailwindUSS.Editor.Tests.csproj --settings Tests/coverlet.runsettings --collect:"XPlat Code Coverage"
```

Coverage reports are written under `Tests/TailwindUSS.Editor.Tests/TestResults/`.

## Documentation

- `docs/tailwind-uss-feature-matrix-ja.md`: Tailwind CSS と Unity USS の対応可能範囲、および実装済み/未実装の一覧
- `docs/tailwind-uss-implementation-plan-ja.md`: 未実装機能を実装するための詳細プラン

## Configuration

Create `tailwinduss.config.json` at the Unity project root.

```json
{
  "inputGlobs": ["Assets/**/*.uxml"],
  "outputUssPath": "Assets/Generated/TailwindUSS.generated.uss",
  "autoAttachGeneratedUss": false
}
```

If the file does not exist, TailwindUSS uses the same defaults in memory. You can also create the file from `Tools/TailwindUSS/Create Default Config`.

## Editor menu

- `Tools/TailwindUSS/Generate`
- `Tools/TailwindUSS/Validate`
- `Tools/TailwindUSS/Create Default Config`

## Currently implemented utility subset

For the full Unity USS coverage matrix and the implementation roadmap for unimplemented utilities, see `docs/tailwind-uss-feature-matrix-ja.md` and `docs/tailwind-uss-implementation-plan-ja.md`.

### Layout

- `flex`
- `hidden`
- `flex-row`
- `flex-col`
- `grow`
- `shrink`

### Alignment

- `items-start`
- `items-center`
- `items-end`
- `justify-start`
- `justify-center`
- `justify-end`
- `justify-between`

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

### Size

- `w-*`
- `h-*`
- `min-w-*`
- `min-h-*`
- `max-w-*`
- `max-h-*`

### Color

- `bg-*`
- `text-*`
- `border-*`

### Typography

- `text-xs`
- `text-sm`
- `text-base`
- `text-lg`
- `font-normal`
- `font-bold`

### Border

- `border`
- `border-0`
- `border-2`

### Radius

- `rounded-none`
- `rounded-sm`
- `rounded`
- `rounded-md`
- `rounded-lg`
- `rounded-full`

## Token scales

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
- `gray-100`
- `gray-300`
- `gray-500`
- `gray-700`
- `gray-900`
- `blue-500`
- `red-500`
- `green-500`
- `yellow-500`

### Font sizes

| token | value |
| --- | --- |
| xs | 12px |
| sm | 14px |
| base | 16px |
| lg | 18px |

## Example

UXML:

```xml
<ui:VisualElement class="flex flex-row items-center px-4 py-2 bg-blue-500 rounded">
    <ui:Label class="text-sm text-white" text="Play" />
</ui:VisualElement>
```

Generated USS:

```css
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

## Package structure

- `package.json`: Unity package manifest
- `Editor/TailwindUSS.Editor.asmdef`: editor assembly
- `Editor/*.cs`: config loading, scanning, resolving, emitting, and menu commands

## Notes

- Unsupported utilities and invalid values are reported as warnings.
- Duplicate tokens in a single `class` attribute are reported as warnings.
- Parse failures, invalid config JSON, and write failures are reported as errors.
- `autoAttachGeneratedUss` adds a matching `Style` element to scanned UXML files when enabled.
