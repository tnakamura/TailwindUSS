# Tailwind CSS と Unity USS の対応可能機能一覧

## この文書の目的

この文書は、**Tailwind CSS の機能のうち Unity 2022.3 UI Toolkit / USS で実現可能なもの**を整理し、**TailwindUSS で実装済みか、未実装か**を明示するための一覧である。

- 判定対象は TailwindUSS パッケージ直下の Unity Package Manager 用 package manifest (`src/TailwindUSS/package.json`) が示す **Unity 2022.3** を前提にする。
- 「実現可能」は **USS のプロパティ・疑似クラス・トランジションで表現できる**ことを意味する。
- 「実装済み」は **現行コード (`src/TailwindUSS/Editor/UtilityResolver.cs`) で解決でき、README にも載っている**状態を指す。

### ステータス凡例

- ✅ **実装済み**: 現在の TailwindUSS が解決できる
- 🟡 **USS で実現可能 / 未実装**: Unity USS では表現できるが TailwindUSS は未対応
- ⚪ **USS では一部のみ実現可能**: Tailwind 本家の一部だけを Unity USS に落とし込める
- ❌ **USS では実現困難**: 現行の USS 制約上、Tailwind 互換での実装が難しい

## 現在の実装範囲

現行実装は `src/TailwindUSS/Editor/UtilityResolver.cs` の registry ベース resolver と family ごとの prefix 解決で構成される。

### ✅ 実装済みトークン

| カテゴリ | Tailwind ユーティリティ | USS 変換 |
| --- | --- | --- |
| レイアウト | `flex`, `hidden`, `visible`, `invisible`, `flex-row`, `flex-col`, `grow`, `shrink`, `overflow-*`, `relative`, `absolute`, `top-*`, `right-*`, `bottom-*`, `left-*`, `inset-*`, `inset-x-*`, `inset-y-*`, `z-*`, `opacity-*` | `display`, `visibility`, `flex-direction`, `flex-grow`, `flex-shrink`, `overflow`, `position`, `top/right/bottom/left`, `z-index`, `opacity` |
| 配置 | `items-start`, `items-center`, `items-end`, `items-stretch`, `justify-start`, `justify-center`, `justify-end`, `justify-between`, `justify-around`, `justify-evenly`, `self-*` | `align-items`, `justify-content`, `align-self` |
| 余白 | `p-*`, `px-*`, `py-*`, `pt-*`, `pr-*`, `pb-*`, `pl-*`, `m-*`, `mx-*`, `my-*`, `mt-*`, `mr-*`, `mb-*`, `ml-*`, `gap-*`, `gap-x-*`, `gap-y-*` | `padding-*`, `margin-*`, `gap`, `column-gap`, `row-gap` |
| サイズ | `w-*`, `h-*`, `min-w-*`, `min-h-*`, `max-w-*`, `max-h-*`, `size-*`, `basis-*`, `order-*` | `width`, `height`, `min-*`, `max-*`, `flex-basis`, `order` |
| 色 | `bg-*`, `bg-transparent`, `bg-current`, `text-*`, `border-*`, `border-t-*`, `border-r-*`, `border-b-*`, `border-l-*` | `background-color`, `color`, `border-*-color` |
| 背景 | `bg-cover`, `bg-contain`, `bg-center`, `bg-top`, `bg-bottom`, `bg-left`, `bg-right`, `bg-repeat`, `bg-no-repeat`, `bg-repeat-x`, `bg-repeat-y`, `bg-none`, `bg-*` (設定済み background image alias) | `background-size`, `background-position`, `background-repeat`, `background-image` |
| タイポグラフィ | `text-xs`〜`text-9xl`, `font-thin`〜`font-black`, `font-*` (設定済み font alias), `italic`, `not-italic`, `text-left`, `text-center`, `text-right`, `text-justify`, `whitespace-*`, `uppercase`, `lowercase`, `capitalize`, `normal-case`, `tracking-*`, `leading-*`, `truncate`, `text-ellipsis`, `text-clip`, `break-normal`, `break-all`, `underline`, `line-through`, `no-underline` | `font-size`, `font-weight`, `-unity-font`, `-unity-font-style`, `-unity-text-align`, `white-space`, `text-transform`, `letter-spacing`, `line-height`, `text-overflow`, `word-break`, `overflow`, `text-decoration` |
| ボーダー | `border`, `border-0`, `border-2`, `border-4`, `border-8`, `border-solid`, `border-t`, `border-r`, `border-b`, `border-l` | `border-*-width` |
| 角丸 | `rounded-none`, `rounded-sm`, `rounded`, `rounded-md`, `rounded-lg`, `rounded-full`, `rounded-t*`, `rounded-r*`, `rounded-b*`, `rounded-l*`, `rounded-tl*`, `rounded-tr*`, `rounded-br*`, `rounded-bl*` | `border-*-radius` |
| 変形 | `scale-*`, `rotate-*`, `translate-x-*`, `translate-y-*`, `origin-*` | `scale`, `rotate`, `translate`, `transform-origin` |
| トランジション | `transition`, `transition-colors`, `transition-opacity`, `transition-transform`, `duration-*`, `delay-*`, `ease-linear`, `ease-in`, `ease-out`, `ease-in-out` | `transition-property`, `transition-duration`, `transition-delay`, `transition-timing-function` |
| インタラクション | `cursor-default`, `cursor-pointer`, `cursor-text`, `cursor-move`, `cursor-not-allowed` | `cursor` |

### 実装済みスケール

- spacing / size: `0, 1, 2, 3, 4, 5, 6, 8, 10, 12`
- colors: `white`, `black`, `gray/slate/zinc/neutral/stone/emerald/sky/indigo/pink` の `100/300/500/700/900`, `blue-500`, `red-500`, `green-500`, `yellow-500`
- font sizes: `xs`, `sm`, `base`, `lg`, `xl`, `2xl`, `3xl`, `4xl`, `5xl`, `6xl`, `7xl`, `8xl`, `9xl`
- `tailwinduss.config.json` の `theme.colors` / `theme.spacing` / `theme.fontSizes` で上記 scale を拡張・上書き可能
- `theme.fonts` と `theme.backgroundImages` で asset alias を追加可能
- size / basis special values: `auto`, `1/2`, `full`; `max-w-*` / `max-h-*` は `none` にも対応
- opacity: `0`, `5`, `10`, `20`, `25`, `30`, `40`, `50`, `60`, `70`, `75`, `80`, `90`, `95`, `100`
- z-index: `0`, `10`, `20`, `30`, `40`, `50`, `auto`
- order: `0`〜`12`
- scale: `0`, `50`, `75`, `90`, `95`, `100`, `105`, `110`, `125`, `150`
- rotate: `0`, `1`, `2`, `3`, `6`, `12`, `45`, `90`, `180`
- translate: spacing scale + `1/2`, `full`
- font weight: `thin`, `extralight`, `light`, `normal`, `medium` -> `normal`; `semibold`, `bold`, `extrabold`, `black` -> `bold`
- text decoration: `underline`, `line-through`, `no-underline`
- transition duration / delay: `75`, `100`, `150`, `200`, `300`, `500`, `700`, `1000`

## Tailwind CSS 機能の対応可能一覧

### 1. レイアウト

| Tailwind 機能 | USS での実現性 | TailwindUSS | 補足 |
| --- | --- | --- | --- |
| `flex`, `hidden` | ✅ | ✅ | 実装済み |
| `block`, `inline`, `inline-block`, `contents`, `list-item` | ❌ | ❌ | UI Toolkit は CSS Display 全互換ではない |
| `overflow-hidden`, `overflow-visible`, `overflow-scroll` | ✅ | ✅ | `overflow` に直接対応 |
| `relative`, `absolute` | ✅ | ✅ | `position` に対応 |
| `top-*`, `right-*`, `bottom-*`, `left-*`, `inset-*`, `inset-x-*`, `inset-y-*` | ✅ | ✅ | spacing scale で位置指定に対応 |
| `z-*` | ✅ | ✅ | `z-index` に対応 |
| `visible`, `invisible` | ⚪ | ✅ | `visibility` に変換。レイアウトは保持される |

### 2. Flexbox

| Tailwind 機能 | USS での実現性 | TailwindUSS | 補足 |
| --- | --- | --- | --- |
| `flex-row`, `flex-col`, `grow`, `shrink` | ✅ | ✅ | 実装済み |
| `flex-wrap`, `flex-nowrap`, `flex-wrap-reverse` | ✅ | ✅ | `flex-wrap` に対応 |
| `justify-around`, `justify-evenly` | ✅ | ✅ | USS の `justify-content` で表現 |
| `items-stretch` | ✅ | ✅ | `align-items: stretch` |
| `self-*` | ✅ | ✅ | `align-self` へ変換 |
| `basis-*` | ✅ | ✅ | `flex-basis` へ変換 |
| `order-*` | ✅ | ✅ | 初期実装は数値 scale (`0`〜`12`) に対応 |
| `gap-*`, `gap-x-*`, `gap-y-*` | ✅ | ✅ | `gap` / `row-gap` / `column-gap` を使う |
| `space-x-*`, `space-y-*` | ❌ | ❌ | 子要素 combinator が必要で USS では実装しづらい |

### 3. 余白・サイズ

| Tailwind 機能 | USS での実現性 | TailwindUSS | 補足 |
| --- | --- | --- | --- |
| `p-*`, `px-*`, `py-*`, `pt-*`, `pr-*`, `pb-*`, `pl-*`, `m-*`, `mx-*`, `my-*` | ✅ | ✅ | 実装済み |
| `mt-*`, `mr-*`, `mb-*`, `ml-*` | ✅ | ✅ | side 個別 margin に対応 |
| `w-*`, `h-*`, `min-w-*`, `min-h-*`, `max-w-*`, `max-h-*` | ✅ | ✅ | 実装済み |
| `size-*` | ✅ | ✅ | `width` と `height` の同時設定で実装済み |
| `w-auto`, `h-auto`, `max-w-none`, `max-h-none`, `basis-auto` などの special values | ✅ | ✅ | `auto` / `none` を family ごとに追加実装済み |
| `%` 系 (`w-1/2`, `h-full`, `basis-full`) | ⚪ | ✅ | `1/2` と `full` を size / basis family に追加。Tailwind の分数全体までは未対応 |
| `container` | ❌ | ❌ | Web の viewport / breakpoint 前提で USS と相性が悪い |

### 4. タイポグラフィ

| Tailwind 機能 | USS での実現性 | TailwindUSS | 補足 |
| --- | --- | --- | --- |
| `text-xs`, `text-sm`, `text-base`, `text-lg`, `text-*` color, `font-normal`, `font-bold` | ✅ | ✅ | 実装済み |
| 追加の `text-xl` 以上の font-size scale | ✅ | ✅ | `text-xl`〜`text-9xl` を実装済み |
| `font-thin` ～ `font-black` | ⚪ | ✅ | USS では `normal` / `bold` に集約して近似実装 |
| `italic`, `not-italic` | ✅ | ✅ | `-unity-font-style` に変換 |
| `text-left`, `text-center`, `text-right`, `text-justify` | ✅ | ✅ | `-unity-text-align` に変換 |
| `tracking-*` | ✅ | ✅ | 初期実装は Tailwind 既定の named scale に対応 |
| `leading-*` | ✅ | ✅ | 初期実装は `leading-3`〜`leading-10` に対応 |
| `whitespace-normal`, `whitespace-nowrap` | ✅ | ✅ | `white-space` に対応 |
| `truncate`, `text-ellipsis`, `text-clip` | ✅ | ✅ | `text-overflow` と `overflow` の組み合わせを実装済み |
| `uppercase`, `lowercase`, `capitalize`, `normal-case` | ✅ | ✅ | `text-transform` に対応 |
| `break-normal`, `break-all` | ✅ | ✅ | `word-break` に対応 |
| `font-*` configured aliases | ✅ | ✅ | `theme.fonts` で alias を定義すると `-unity-font` を生成 |
| `underline`, `line-through`, `no-underline` | ⚪ | ✅ | `text-decoration` の基本値に対応 |
| `decoration-*` | ❌ | ❌ | Unity 2022.3 では Tailwind 同等の decoration color / style / thickness は持てない |

### 5. 背景

| Tailwind 機能 | USS での実現性 | TailwindUSS | 補足 |
| --- | --- | --- | --- |
| `bg-*` (color) | ✅ | ✅ | 実装済み |
| 追加色パレット (`slate-*`, `zinc-*`, `emerald-*` など) | ✅ | ✅ | `gray/slate/zinc/neutral/stone/emerald/sky/indigo/pink` の `100/300/500/700/900` を実装済み |
| `bg-transparent`, `bg-current` | ✅ | ✅ | 背景色 special value を実装済み |
| `bg-none`, `bg-cover`, `bg-contain` | ✅ | ✅ | `background-image` / `background-size` に対応 |
| `bg-center`, `bg-top`, `bg-bottom`, `bg-left`, `bg-right` | ✅ | ✅ | `background-position` に対応 |
| `bg-repeat`, `bg-no-repeat`, `bg-repeat-x`, `bg-repeat-y` | ✅ | ✅ | `background-repeat` に対応 |
| `bg-*` configured background image aliases | ✅ | ✅ | `theme.backgroundImages` で alias を定義すると `background-image` を生成 |
| gradient 系 (`bg-gradient-to-*`, `from-*`, `via-*`, `to-*`) | ❌ | ❌ | USS の標準機能だけでは Tailwind gradient を再現できない |

### 6. ボーダー・角丸・効果

| Tailwind 機能 | USS での実現性 | TailwindUSS | 補足 |
| --- | --- | --- | --- |
| `border`, `border-0`, `border-2`, `border-*` color, `rounded*` | ✅ | ✅ | 実装済み |
| `border-4`, `border-8` など追加幅 | ✅ | ✅ | `border-4`, `border-8` を実装済み |
| `border-t-*`, `border-r-*`, `border-b-*`, `border-l-*` | ✅ | ✅ | side ごとの幅・色へ展開する resolver を実装済み |
| `rounded-t-*`, `rounded-r-*`, `rounded-b-*`, `rounded-l-*`, corner 個別 | ✅ | ✅ | side / corner ごとの radius 生成を実装済み |
| `border-solid` | ⚪ | ✅ | USS は実質 `solid` のみのため no-op utility として扱う |
| `opacity-*` | ✅ | ✅ | Tailwind 既定 opacity scale に対応 |
| `outline-*`, `ring-*`, `ring-offset-*` | ❌ | ❌ | USS に Tailwind 相当の outline / ring 機構がない |
| `shadow-*`, `drop-shadow-*` | ❌ | ❌ | USS に box-shadow / filter がない |

### 7. 変形・トランジション・状態

| Tailwind 機能 | USS での実現性 | TailwindUSS | 補足 |
| --- | --- | --- | --- |
| `scale-*`, `rotate-*`, `translate-x-*`, `translate-y-*`, `origin-*` | ✅ | ✅ | `scale`, `rotate`, `translate`, `transform-origin` を生成 |
| `transition`, `transition-*`, `duration-*`, `delay-*`, `ease-*` | ✅ | ✅ | `transition-property`, `transition-duration`, `transition-delay`, `transition-timing-function` を生成 |
| `hover:`, `active:`, `focus:`, `disabled:`, `checked:`, `selected:` | ✅ | ✅ | USS 疑似クラスへ変換して selector を生成 |
| 複合 variant (`hover:focus:` など) | ✅ | ✅ | selector suffix を左から順に連結して生成 |
| `group-hover:`, `peer-*`, `first:`, `last:`, `odd:`, `even:` | ❌ | ❌ | combinator / structural pseudo class が弱く Tailwind 同等は難しい |
| `animate-*` | ❌ | ❌ | USS transition はあるが Tailwind keyframes 相当はない |

### 8. インタラクション

| Tailwind 機能 | USS での実現性 | TailwindUSS | 補足 |
| --- | --- | --- | --- |
| `cursor-pointer`, `cursor-default`, `cursor-text` など | ✅ | ✅ | `cursor-default`, `cursor-pointer`, `cursor-text`, `cursor-move`, `cursor-not-allowed` を実装済み |
| `select-none`, `select-text` | ❌ | ❌ | `user-select` が USS にない |
| `pointer-events-none`, `pointer-events-auto` | ❌ | ❌ | USS ではなく C# の `pickingMode` 管理が必要 |
| `accent-*`, `appearance-*`, `resize`, `scroll-*`, `touch-*`, `will-change` | ❌ | ❌ | Unity USS の責務外、または未対応 |

## 優先度付きロードマップ

feature matrix 上で『USS で実現可能 / 未実装』としていた utility は実装完了。

今後の拡張候補として残るのは、matrix の対象外としていた arbitrary な background image utilities など、追加仕様を伴うものに限られる。

## Unity 6.3+ を前提にした再評価

現行パッケージの package manifest は `Unity 2022.3` を対象にしているため、以下は **サポート対象を Unity 6.3+ に引き上げる場合の追加候補**として整理する。

Unity 6.3 では UI Toolkit / USS に built-in `filter` と custom filter の導線が追加されており、**Tailwind の filter 系 utility の一部**は 2022.3 前提より現実的になる。

| 項目 | Unity 2022.3 | Unity 6.3+ | Tailwind utility 候補 | 補足 |
| --- | --- | --- | --- | --- |
| built-in `filter` | ❌ | 🟡 | `blur-*`, `grayscale`, `grayscale-0`, `invert`, `invert-0`, `sepia`, `sepia-0`, `contrast-*`, `hue-rotate-*` | Unity 6.3 の built-in filters で直接マッピングしやすい |
| `filter` の transition | ⚪ | 🟡 | 既存の `transition`, `duration-*`, `delay-*`, `ease-*` と併用 | filter 値の変化は既存 variant と相性がよい。`hover:blur-sm` のような組み合わせが候補 |
| custom filter | ❌ | ⚪ | 直接対応よりも将来拡張候補 | shader / asset 依存が強く、Tailwind の標準 utility とは 1:1 にしづらい |
| `drop-shadow-*`, `shadow-*` | ❌ | ❌ | なし | built-in filter 群に `drop-shadow()` は見当たらず、box-shadow 相当も依然弱い |
| `backdrop-blur-*`, `backdrop-*` | ❌ | ❌ | なし | 標準の built-in filter だけで Tailwind の backdrop 系をそのまま再現しづらい |
| `brightness-*`, `saturate-*` | ❌ | ❌ | 当面なし | Unity 6.3 の built-in filter 一覧に標準対応が見当たらない |

### Unity 6.3+ で優先度が上がる utility

低コスト順に並べると、まず着手候補になるのは次の順序である。

1. `blur-*`
2. `grayscale`, `grayscale-0`
3. `invert`, `invert-0`
4. `sepia`, `sepia-0`
5. `contrast-*`
6. `hue-rotate-*`

### Unity 6.3+ でも残る制約

- `filter` 系 utility は **複数 token を 1 つの `filter` 宣言へ合成する設計**が必要になる。
- `shadow-*` / `drop-shadow-*` / `ring-*` / `outline-*` は、6.3 にしてもそのまま Tailwind 互換にはなりにくい。
- custom filter を採用すると shader / asset / pipeline 依存が発生し、現行の「設定ファイル中心で完結する」設計から外れやすい。

## 実装しない前提でよいもの

以下は Tailwind 本家にはあるが、**Unity USS を前提にする限り TailwindUSS の対象外として整理した方がよい**。

- Grid 系
- Space / divide / peer / group 系
- Responsive variants (`sm:`, `md:` など)
- Container queries
- Filters / backdrop filters（※Unity 2022.3 前提では対象外。Unity 6.3+ の built-in filter は別途再評価対象）
- Shadow / ring / outline
- Blend modes
- Tables
- SVG utilities
- Accessibility utilities (`sr-only` など)

## 根拠として見た実装箇所

- `src/TailwindUSS/Editor/UtilityResolver.cs`
- `test/TailwindUSS.Editor.Tests/UtilityResolverTests.cs`
- `README.md`
