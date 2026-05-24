# TailwindUSS

[English](README.md) | 日本語

TailwindUSS は、UI Toolkit プロジェクト向けの Unity Editor 拡張です。UXML 内の `class` 属性を走査し、Tailwind 風の utility の一部を解決して、実際に使用されている utility だけを含む単一の USS ファイルを生成します。

## 主な機能

- 設定した glob パターンに一致する UXML を走査
- 使用されている utility token だけを USS として生成
- 未対応 utility、未対応 variant、不正な値、重複 token を検証
- `hover:` や `hover:focus:` のような pseudo-class variant に対応
- 必要に応じて生成された `<Style src="..." />` を走査対象 UXML に自動追加
- 対象 UXML 保存時の USS 自動再生成を任意で有効化可能
- 組み込みの color / spacing / font-size scale を設定で拡張・上書き可能
- font alias と background image alias を設定可能

## 動作要件

- Unity 6.3 以降
- 現在の Unity USS 互換表と残タスクは `docs/tailwind-uss-feature-matrix-ja.md` / `docs/tailwind-uss-implementation-plan-ja.md` を参照してください。

## インストール

このリポジトリには `src/TailwindUSS` 配下に Unity Package Manager パッケージが含まれています。

プロジェクトの `Packages/manifest.json` に次を追加してください。

```json
{
  "dependencies": {
    "com.github-tnakamura.tailwinduss": "https://github.com/tnakamura/TailwindUSS.git?path=/src/TailwindUSS"
  }
}
```

同じ Git URL は Unity Package Manager の UI から追加しても構いません。

## クイックスタート

1. パッケージをインストールする
2. Unity Editor の `Project Settings > TailwindUSS` で設定する、または Unity プロジェクトルートに `tailwinduss.config.json` を作成 / `Tools/TailwindUSS/Create Default Config` を実行する
3. UXML の `class` 属性に utility class を記述する
4. `Tools/TailwindUSS/Generate` を実行する
5. `outputUssPath` に出力された USS を利用する

## サンプルプロジェクト

Unity 6.3 向けのサンプルプロジェクトを `samples/TailwindUSS.Sample` に追加しました。`file:../../src/TailwindUSS` でこのリポジトリを参照し、すぐに開ける UXML 例と生成済み USS を含みます。

## 設定

`Project Settings > TailwindUSS` から設定するか、Unity プロジェクトルートに `tailwinduss.config.json` を配置します。

```json
{
  "inputGlobs": ["Assets/**/*.uxml"],
  "outputUssPath": "Assets/Generated/TailwindUSS.generated.uss",
  "autoAttachGeneratedUss": false,
  "autoGenerateOnUxmlSave": false,
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

設定ファイルが存在しない場合、TailwindUSS はメモリ上のデフォルト設定を使い、Unity Editor に warning を出します。`Project Settings > TailwindUSS` から保存するか、`Tools/TailwindUSS/Create Default Config` から既定ファイルを生成できます。

### 設定項目

- `inputGlobs`: 走査対象の UXML。空の場合は実質的に `Assets/**/*.uxml` が既定値です。
- `outputUssPath`: 生成される USS の相対パスまたは絶対パス。
- `autoAttachGeneratedUss`: `true` の場合、対象 UXML の先頭に同じ `src` を持つ `Style` 要素がなければ追加します。
- `autoGenerateOnUxmlSave`: `true` の場合、対象 UXML 保存後に USS を自動再生成します。未変更ファイルの走査結果はキャッシュし、変更された UXML だけを再解析します。
- `theme.colors`: 組み込み color scale の拡張・上書き。
- `theme.spacing`: 組み込み spacing / size token の拡張・上書き。
- `theme.fontSizes`: 組み込み font size の拡張・上書き。
- `theme.fonts`: `font-*` alias を USS の asset reference にマップします。通常の文字列は `resource("...")` として出力され、`resource(...)` と `url(...)` はそのまま使われます。
- `theme.backgroundImages`: `bg-*` alias を `background-image` にマップし、asset reference の扱いは `theme.fonts` と同じです。

## Editor メニュー

- `Tools/TailwindUSS/Generate`
- `Tools/TailwindUSS/Validate`
- `Tools/TailwindUSS/Create Default Config`
- `Tools/TailwindUSS/Settings`

## Diagnostics の挙動

TailwindUSS は Unity Editor 上で次を報告します。

- 未対応 utility に対する warning
- 未対応 variant に対する warning
- 不正な scale / token 値に対する warning
- 同一 `class` 属性内の重複 token に対する warning
- config 読み込み失敗に対する error
- UXML parse 失敗に対する error
- USS 書き込み失敗に対する error
- 自動 style 参照更新失敗に対する error

## 対応 variant

- `hover:`
- `active:`
- `focus:`
- `disabled:`
- `checked:`
- `selected:`
- `hover:focus:` のような複合 variant

## 現在実装されている utility subset

Unity USS とのより広い互換表と、未実装 utility のロードマップは次を参照してください。

- `docs/tailwind-uss-feature-matrix-ja.md`
- `docs/tailwind-uss-implementation-plan-ja.md`
- 上記 2 文書には、今回実装した Unity 6.3+ 向け filter 系 utility と残タスクの整理も含まれます。

### レイアウト

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

### 配置

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

### 余白

Unity USS には `gap`, `row-gap`, `column-gap` がなく、UI Toolkit USS には厳密な polyfill に必要な structural selector もないため、gap utility は除外しています。単純なレイアウトでは子要素に margin utility を付けて代替してください。

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

### サイズ

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

### 色

- `bg-*`
- `bg-transparent`
- `bg-current`
- `text-*`
- `border-*`
- `border-t-*`
- `border-r-*`
- `border-b-*`
- `border-l-*`

### 背景

- `bg-*`（設定済み background image alias）
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

### タイポグラフィ

Unity USS には `text-transform` がなく、他の USS プロパティでも代替できないため、文字ケース変更ユーティリティは意図的に除外しています。対応するには `text` / `value` 自体の書き換えが必要です。

leading utility も Unity USS に `line-height` がないため除外しています。`-unity-paragraph-spacing` は段落区切りにしか効かず、Tailwind の行送りは再現できません。

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
- `font-*`（設定済み font alias）
- `text-left`
- `text-center`
- `text-right`
- `text-justify`
- `italic`
- `not-italic`
- `whitespace-normal`
- `whitespace-nowrap`
- `tracking-tighter`
- `tracking-tight`
- `tracking-normal`
- `tracking-wide`
- `tracking-wider`
- `tracking-widest`
- `truncate`
- `text-ellipsis`
- `text-clip`
- `break-normal`
- `break-all`
- `underline`
- `line-through`
- `no-underline`

### ボーダー

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

### 角丸

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

### 変形

- `scale-*`
- `rotate-*`
- `translate-x-*`
- `translate-y-*`
- `origin-*`

### フィルター

- `blur-none`, `blur-sm`, `blur`, `blur-md`, `blur-lg`, `blur-xl`, `blur-2xl`, `blur-3xl`
- `grayscale`, `grayscale-0`
- `invert`, `invert-0`
- `sepia`, `sepia-0`
- `contrast-0`, `contrast-50`, `contrast-75`, `contrast-100`, `contrast-125`, `contrast-150`, `contrast-200`
- `hue-rotate-0`, `hue-rotate-15`, `hue-rotate-30`, `hue-rotate-60`, `hue-rotate-90`, `hue-rotate-180`
- 同じ要素に複数の filter utility を付けた場合は、1 つの合成 `filter` 宣言として出力します。同一 family が重複した場合は warning を出し、最後の token を採用します。

### トランジション

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

### インタラクション

Unity の cursor keyword は CSS と異なり、`cursor-default` は `arrow`、`cursor-pointer` は `link`、`cursor-move` は `move-arrow` に対応します。

- `cursor-default`
- `cursor-pointer`
- `cursor-text`
- `cursor-move`
- `cursor-not-allowed`

## Token スケール

`theme.colors`、`theme.spacing`、`theme.fontSizes` により、以下の組み込み値を拡張・上書きできます。

### spacing / size スケール

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

### 色

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

`gray`、`slate`、`zinc`、`neutral`、`stone`、`emerald`、`sky`、`indigo`、`pink` は `100`、`300`、`500`、`700`、`900` に対応します。

### フォントサイズ

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

### その他の組み込み scale

- `size-*`, `w-*`, `h-*`, `basis-*`: spacing scale に `auto`, `1/2`, `full` を追加
- `min-w-*`, `min-h-*`: spacing scale に `1/2`, `full` を追加
- `max-w-*`, `max-h-*`: spacing scale に `1/2`, `full`, `none` を追加
- `font-*` weight: `thin`, `extralight`, `light`, `normal`, `medium` は `-unity-font-style: normal`、`semibold`, `bold`, `extrabold`, `black` は `-unity-font-style: bold` に集約し、`italic` と併用時は `bold-and-italic` を合成
- `text-decoration`: `underline`, `line-through`, `no-underline`
- `opacity-*`: `0`, `5`, `10`, `20`, `25`, `30`, `40`, `50`, `60`, `70`, `75`, `80`, `90`, `95`, `100`
- `z-*`: `0`, `10`, `20`, `30`, `40`, `50`, `auto`
- `order-*`: `0` 〜 `12`
- `tracking-*`: `tighter`, `tight`, `normal`, `wide`, `wider`, `widest`
- `border-*` の width: `0`, `2`, `4`, `8`。加えて `border` shorthand は `1px`
- radius 値: `none`, `sm`, default, `md`, `lg`, `full`
- `scale-*`: `0`, `50`, `75`, `90`, `95`, `100`, `105`, `110`, `125`, `150`
- `rotate-*`: `0`, `1`, `2`, `3`, `6`, `12`, `45`, `90`, `180`
- `translate-x-*`, `translate-y-*`: spacing scale に `1/2` と `full` を追加
- `origin-*`: `center`, `top`, `top-right`, `right`, `bottom-right`, `bottom`, `bottom-left`, `left`, `top-left`
- `duration-*`, `delay-*`: `75`, `100`, `150`, `200`, `300`, `500`, `700`, `1000`
- `blur-*`: `none`, `sm`, default, `md`, `lg`, `xl`, `2xl`, `3xl`
- `contrast-*`: `0`, `50`, `75`, `100`, `125`, `150`, `200`
- `hue-rotate-*`: `0`, `15`, `30`, `60`, `90`, `180`

## 例

UXML:

```xml
<ui:VisualElement class="flex flex-row items-center px-4 py-2 bg-blue-500 rounded">
    <ui:Label class="text-sm text-white" text="Play" />
</ui:VisualElement>
```

生成される USS:

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

## 開発

### 自動テスト

リポジトリルートで次を実行します。

```bash
dotnet test test/TailwindUSS.Editor.Tests/TailwindUSS.Editor.Tests.csproj
dotnet test test/TailwindUSS.Editor.Unit/TailwindUSS.Editor.Unit.csproj
```

Cobertura 形式で coverage を取得する場合:

```bash
dotnet test test/TailwindUSS.Editor.Tests/TailwindUSS.Editor.Tests.csproj --settings test/coverlet.runsettings --collect:"XPlat Code Coverage"
```

coverage レポートは `test/TailwindUSS.Editor.Tests/TestResults/` 以下に出力されます。

## パッケージ構成

- `src/TailwindUSS/package.json`: Unity package manifest
- `src/TailwindUSS/Editor/TailwindUSS.Editor.asmdef`: editor 専用 assembly definition
- `src/TailwindUSS/Editor/*.cs`: config 読み込み、走査、parse、resolve、emit、validation、generation、menu command

## ドキュメント

追加ドキュメントは現在日本語で管理されています。

- `docs/tailwind-uss-feature-matrix-ja.md`: Tailwind CSS / Unity USS の互換表と実装状況
- `docs/tailwind-uss-implementation-plan-ja.md`: 未対応 utility の実装ロードマップ
- `docs/mvp-spec-ja.md`: MVP 仕様
- `docs/implementation-approaches-ja.md`: 設計メモと実装アプローチ

## ライセンス

MIT
