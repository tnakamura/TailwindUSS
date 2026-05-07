# TailwindUSS 未実装機能 実装プラン

## 目的

この文書は、`tailwind-uss-feature-matrix-ja.md` で **USS では実現可能だが TailwindUSS では未実装** と判定した機能を、**AI がそのまま実装作業に着手できる粒度**まで分解した実装計画である。

対象 Unity バージョンは、TailwindUSS パッケージ直下の Unity Package Manager 用 package manifest (`package.json`) に合わせて **Unity 2022.3** とする。

> 進捗更新: 先行設計改善と Phase 1-4 は実装完了。

## 完了条件

次の条件をすべて満たしたときに、この計画は完了したとみなす。

1. `README.md` の「Currently implemented utility subset」が実装に追随して更新されている。
2. `ValidationService` が新規 utility / variant の unsupported / invalid value を正しく診断できる。
3. `GenerationService` が追加 utility を USS に出力できる。
4. `Tests/TailwindUSS.Editor.Tests/UtilityResolverTests.cs` に新規 utility family の正常系・異常系テストがある。
5. `Tests/TailwindUSS.Editor.Tests/GenerationServiceTests.cs` に end-to-end の生成確認がある。
6. 追加した selector 記法がある場合、`Tests/TailwindUSS.Editor.Tests/UssEmitterTests.cs` に selector 出力の回帰テストがある。

## 現在のアーキテクチャ

- `Editor/UxmlScanner.cs`
  - `class` 属性を収集する。
  - 現状は **生トークン列**のみを返す。
- `Editor/ClassTokenParser.cs`
  - 空白区切り・重複除去のみを担当する。
  - `hover:bg-blue-500` のような variant を理解しない。
- `Editor/UtilityResolver.cs`
  - 実装の中心。
  - 固定トークンは `switch`、可変トークンは prefix 走査で解決する。
- `Editor/UssEmitter.cs`
  - `.token { ... }` をそのまま出力する。
  - 現状は pseudo class suffix や特殊文字 escaping を持たない。
- `Editor/ValidationService.cs` / `Editor/GenerationService.cs`
  - resolver の戻り値を使って warning / error を記録する。

## 先に入れるべき設計改善

未実装機能をこのまま `switch` に足し続けると保守不能になるため、**機能追加の前に最小限の拡張基盤を入れる**。

### 1. Resolver を registry 化する ✅ 完了

### 変更対象

- `Editor/UtilityResolver.cs`
- `Editor/TailwindUssModels.cs`
- `Tests/TailwindUSS.Editor.Tests/UtilityResolverTests.cs`

### 実施内容

1. 固定トークンを `Dictionary<string, Func<ResolvedUtility>>` に移す。
2. prefix utility を `IUtilityHandler` 相当の内部 private 型、または `TryResolveXxx` 群を family 単位で整理したテーブルへ寄せる。
3. 以下の scale / special value を辞書として切り出す。
   - spacing
   - sizing special values (`auto`, `full`, `none` など)
   - opacity
   - z-index
   - border width
   - radius
   - transition duration / delay
   - easing
4. `ResolvedUtility` が将来的に selector suffix を持てるよう、`SelectorSuffix` 文字列プロパティを追加する。
   - 現段階では空文字でよい。

### 受け入れ条件

- 既存 75 テストがすべて通る。
- 既存 utility の出力が 1 つも変わらない。

## 実装フェーズ

## Phase 1: 既存 parser のまま増やせる utility を追加する ✅ 完了

variant や特殊 selector を伴わない、**単純な property マッピング**から着手する。

### 1-1. Layout / positioning ✅ 完了

### 追加対象 utility

- `overflow-hidden`, `overflow-visible`, `overflow-scroll`
- `relative`, `absolute`
- `top-*`, `right-*`, `bottom-*`, `left-*`
- `inset-*`, `inset-x-*`, `inset-y-*`
- `z-0`, `z-10`, `z-20`, `z-30`, `z-40`, `z-50`, `z-auto`
- `opacity-*`

### 実装方法

1. `UtilityResolver` に以下の handler を追加する。
   - `TryResolveOverflow`
   - `TryResolvePosition`
   - `TryResolveInset`
   - `TryResolveZIndex`
   - `TryResolveOpacity`
2. inset 系は既存 spacing scale を再利用し、`top/right/bottom/left` の複数宣言生成に合わせる。
3. `absolute` / `relative` は固定トークン扱いでよい。
4. `opacity-*` は `0`〜`100` を 5 または 10 刻みの Tailwind 既定値に限定し、`0.0`〜`1.0` へ変換する。

### テスト追加

- `UtilityResolverTests`
  - 正常系: 各 family で最低 1 件ずつ
  - 異常系: `top-7`, `z-15`, `opacity-73` など unsupported / invalid
- `GenerationServiceTests`
  - `absolute top-0 right-0 opacity-50 overflow-hidden` を含む UXML を生成させる

実装済み:

- `overflow-*`, `relative`, `absolute`, `top/right/bottom/left-*`, `inset*`, `z-*`, `opacity-*`
- `UtilityResolverTests` と `GenerationServiceTests` に正常系・異常系を追加済み

### 1-2. Flex container / child の拡張 ✅ 完了

### 追加対象 utility

- `flex-wrap`, `flex-nowrap`, `flex-wrap-reverse`
- `items-stretch`
- `justify-around`, `justify-evenly`
- `self-auto`, `self-start`, `self-end`, `self-center`, `self-stretch`
- `basis-*`
- `order-*`
- `gap-*`, `gap-x-*`, `gap-y-*`
- `mt-*`, `mr-*`, `mb-*`, `ml-*`

### 実装方法

1. margin family に side 個別定義を追加する。
2. `gap-*` は spacing scale を使って `gap`, `column-gap`, `row-gap` に展開する。
3. `basis-*` は size family と同じ値パーサーを再利用する。
4. `order-*` は整数値 scale を別辞書で持つ。まずは Tailwind 既定の `first/last/none` を除き、数値系から始める。
5. `self-*` / `items-stretch` / `justify-around` / `justify-evenly` は固定マッピングで足せる。

### テスト追加

- `UtilityResolverTests` に各トークンの property / value アサートを追加
- `UssEmitterTests` に `gap-x-*` / `gap-y-*` の multi-declaration 出力確認を追加

実装済み:

- `flex-wrap*`, `items-stretch`, `justify-around`, `justify-evenly`, `self-*`, `basis-*`, `order-*`, `gap*`, `mt/mr/mb/ml`
- resolver / emitter / generation の回帰テストを追加済み

### 1-3. Typography の拡張 ✅ 完了

### 追加対象 utility

- `text-left`, `text-center`, `text-right`, `text-justify`
- `italic`, `not-italic`
- `whitespace-normal`, `whitespace-nowrap`
- `uppercase`, `lowercase`, `capitalize`, `normal-case`
- `tracking-*`
- `leading-*`
- `truncate`, `text-ellipsis`, `text-clip`
- `break-normal`, `break-all`
- 追加 font-size scale (`xl`, `2xl`, `3xl` ...)

### 実装方法

1. `TryResolveFontSize` を scale 辞書化し、色 family と衝突しない順序を維持する。
2. `tracking-*` と `leading-*` 用の新規 scale を追加する。
3. `truncate` は 1 トークンで次の複数宣言を返す。
   - `overflow: hidden`
   - `text-overflow: ellipsis`
   - `white-space: nowrap`
4. `text-ellipsis` / `text-clip` は `text-overflow` 単体にする。
5. `italic` / `not-italic` は Unity 2022.3 で有効な font style property に寄せる。
6. `font-*` weight を増やす場合は、フォントアセット依存であることを README に明記する。

### テスト追加

- `UtilityResolverTests` に `truncate` の複数宣言検証を追加
- `GenerationServiceTests` で `truncate tracking-wide leading-6` の end-to-end を追加

実装済み:

- `text-left/right/center/justify`, `italic`, `whitespace-*`, `text-transform`, `tracking-*`, `leading-*`
- `truncate`, `text-ellipsis`, `text-clip`, `break-*`, `text-xl`〜`text-9xl`
- `UtilityResolverTests` と `GenerationServiceTests` に typography 回帰テストを追加済み

## Phase 2: 背景・ボーダーの拡張 ✅ 完了

### 2-1. Border / radius の拡張 ✅ 完了

### 追加対象 utility

- `border-4`, `border-8`
- `border-t`, `border-r`, `border-b`, `border-l`
- `border-t-*`, `border-r-*`, `border-b-*`, `border-l-*` color / width
- `rounded-t-*`, `rounded-r-*`, `rounded-b-*`, `rounded-l-*`
- corner 個別 (`rounded-tl-*`, `rounded-tr-*`, `rounded-br-*`, `rounded-bl-*`)

### 実装方法

1. side ごとの property name 配列を定数化する。
2. width family と color family を side-aware に拡張する。
3. radius family は既存 `RadiusProperties` を再利用しつつ、対象 corner 配列を family ごとに分ける。

### テスト追加

- width / color / radius の side-specific utility を追加
- `rounded-t-lg` などが期待する 2 corner のみに出力されることを確認

実装済み:

- `border-4`, `border-8`, `border-t/r/b/l`, `border-t/r/b/l-*` の width / color
- `rounded-t/r/b/l*`, `rounded-tl/tr/br/bl*`
- `UtilityResolverTests` と `GenerationServiceTests` にボーダー・角丸回帰テストを追加済み

### 2-2. Background の拡張 ✅ 完了

### 追加対象 utility

- 追加 palette (`slate`, `zinc`, `neutral`, `stone`, `emerald`, `sky`, `indigo`, `pink` など)
- `bg-transparent`, `bg-current`
- `bg-cover`, `bg-contain`
- `bg-center`, `bg-top`, `bg-bottom`, `bg-left`, `bg-right`
- `bg-repeat`, `bg-no-repeat`, `bg-repeat-x`, `bg-repeat-y`
- `bg-none`

### 実装方法

1. `Colors` を Tailwind 既定 palette の必要分まで拡張する。
2. background size / repeat / position は固定トークン辞書で実装する。
3. `bg-none` は `background-image: none` を返す。
4. 背景画像 asset 指定はこの Phase では入れず、Phase 5 の設定拡張に回す。

### テスト追加

- palette 拡張と background image related fixed tokens の検証

実装済み:

- `gray/slate/zinc/neutral/stone/emerald/sky/indigo/pink` の `100/300/500/700/900`
- `bg-transparent`, `bg-current`, `bg-cover`, `bg-contain`, `bg-center/top/bottom/left/right`
- `bg-repeat`, `bg-no-repeat`, `bg-repeat-x`, `bg-repeat-y`, `bg-none`
- `UtilityResolverTests` と `GenerationServiceTests` に背景回帰テストを追加済み

## Phase 3: variant 対応 ✅ 完了

ここからは `ClassTokenParser` と `UssEmitter` の改修が必要になる。

### 3-1. 対応する variant の初期セット

- `hover:`
- `active:`
- `focus:`
- `disabled:`
- `checked:`
- `selected:`

### 3-2. 変更対象

- `Editor/ClassTokenParser.cs`
- `Editor/TailwindUssModels.cs`
- `Editor/UtilityResolver.cs`
- `Editor/UssEmitter.cs`
- `Tests/TailwindUSS.Editor.Tests/ClassTokenParserTests.cs`
- `Tests/TailwindUSS.Editor.Tests/UssEmitterTests.cs`
- `Tests/TailwindUSS.Editor.Tests/GenerationServiceTests.cs`

### 3-3. データモデル変更

1. `UxmlTokenOccurrence` に以下を追加する。
   - `OriginalToken`
   - `VariantChain` (`IList<string>`)
   - `BaseToken`
2. `ResolvedUtility` に以下を追加する。
   - `SelectorSuffix`
   - 必要なら `EscapedClassName`

### 3-4. Parser 実装ルール

1. `hover:bg-blue-500` は `VariantChain = ["hover"]`, `BaseToken = "bg-blue-500"` に分解する。
2. `hover:focus:bg-blue-500` は左から順に variant を積む。
3. UXML 上の class 名は `hover:bg-blue-500` のような**元トークン文字列をそのまま保持**する。parser は内部表現だけを `VariantChain` と `BaseToken` に分解する。
4. 現段階では escape syntax (`\:`) を解釈しなくてよい。TailwindUSS の class 記法では colon を variant separator 専用にする。
5. 未知 variant は `Unsupported utility token` ではなく、**variant unsupported** として warning を出せるよう issue kind を追加してよい。

### 3-5. Resolver / Emitter 実装ルール

1. resolver は **base token を通常解決**したあと、variant を selector suffix へ変換する。
2. variant suffix マップ:
   - `hover` -> `:hover`
   - `active` -> `:active`
   - `focus` -> `:focus`
   - `disabled` -> `:disabled`
   - `checked` -> `:checked`
   - `selected` -> `:selected`
3. emitter は class selector の token 部分を **USS selector として安全に escape** する。
   - `hover:bg-blue-500` をそのまま class 名に使う場合、出力 selector は `.hover\:bg-blue-500:hover` のようにする。
4. utilities の一意性は **元トークン文字列**で管理する。`bg-blue-500` と `hover:bg-blue-500` は別ルールとして出力する。

### 3-6. テスト追加

- parser: `hover:bg-blue-500 hover:bg-blue-500 focus:text-white`
- emitter: escaped class name + pseudo class suffix
- generation: variant token を含む UXML から正しい USS が出ること

実装済み:

- `hover:`, `active:`, `focus:`, `disabled:`, `checked:`, `selected:` と複合 variant の parser / resolver / emitter 対応
- `hover:bg-blue-500` のような class token を保持したまま、`.hover\:bg-blue-500:hover` 形式で USS selector を出力
- 未対応 variant を `UnsupportedVariant` warning として診断
- `ClassTokenParserTests`, `UtilityResolverTests`, `UssEmitterTests`, `GenerationServiceTests` に variant 回帰テストを追加済み

## Phase 4: transform / transition / cursor ✅ 完了

### 追加対象 utility

- `scale-*`
- `rotate-*`
- `translate-x-*`, `translate-y-*`
- `origin-*`
- `transition`, `transition-colors`, `transition-opacity`, `transition-transform`
- `duration-*`, `delay-*`, `ease-linear`, `ease-in`, `ease-out`, `ease-in-out`
- `cursor-default`, `cursor-pointer`, `cursor-text`, `cursor-move`, `cursor-not-allowed`

### 実装方法

1. transform 系は新しい数値 / angle / percentage scale を用意する。
2. `translate-x-*` / `translate-y-*` は、既存値と合成する仕様を持たせない。MVP では `translate` をその token 単体で完結させる。
   - 例: `translate-x-4` -> `translate: 16px 0`
3. `scale-*` も MVP では uniform scale のみ先に入れる。
4. transition 系は property family と duration / easing / delay を別 token で出す。
5. cursor 系は fixed mapping でよい。

### テスト追加

- resolver: transform / transition / cursor の単体テスト
- generation: `hover:scale-105 transition duration-150 ease-out cursor-pointer`

実装済み:

- `scale-*`, `rotate-*`, `translate-x-*`, `translate-y-*`, `origin-*`
- `transition`, `transition-colors`, `transition-opacity`, `transition-transform`
- `duration-*`, `delay-*`, `ease-linear`, `ease-in`, `ease-out`, `ease-in-out`
- `cursor-default`, `cursor-pointer`, `cursor-text`, `cursor-move`, `cursor-not-allowed`
- `UtilityResolverTests` と `GenerationServiceTests` に Phase 4 回帰テストを追加済み

## Phase 5: 設定ファイル拡張と asset 依存 utility

### 対象

- font family aliases
- background image aliases
- palette / spacing / typography scale のユーザー拡張

### 変更対象

- `Editor/TailwindUssConfig.cs`
- `Editor/ConfigLoader.cs`
- `README.md`
- config 関連テスト

### 実装方法

1. config JSON に以下の optional セクションを追加する。
   - `theme.colors`
   - `theme.spacing`
   - `theme.fontSizes`
   - `theme.fonts`
   - `theme.backgroundImages`
2. 既定値があるものは現行のハードコード辞書を default theme として流用する。
3. alias 解決順は `user config > built-in defaults` に固定する。
4. config 不正時の診断メッセージを既存スタイルに合わせる。

## 実装時の注意点

### 1. prefix 衝突順序

`text-*` は **font-size family** と **text color family** が衝突する。現状は font-size 判定を先に行い、その後 color 判定しているため、以下を維持する。

1. `text-xs`, `text-sm`, `text-base`, `text-lg`, `text-xl` など size scale を先に判定
2. size scale に無い `text-white`, `text-blue-500` などを color として判定

### 2. multi-declaration utility の重複

`truncate`, `inset-x-*`, `gap-*`, `rounded-t-*` などは複数宣言を返す。`ResolvedUtility.Declarations` の順序は **テストで固定**し、emit 出力の安定性を維持する。

### 3. 既存診断との整合

- family は分かるが値が不正 -> `InvalidValue`
- family 自体が未対応 -> `Unsupported`
- variant だけ未対応 -> 新しい issue kind を追加するか、明示メッセージ付き Warning にする

### 4. USS 非対応機能は入れない

次は計画対象外。

- Grid
- Space / divide
- peer / group / structural variants
- ring / outline / shadow / filters
- pointer-events / user-select
- responsive variants

## テスト実行順

各フェーズで最低限、次を回す。

```bash
dotnet test Tests/TailwindUSS.Editor.Tests/TailwindUSS.Editor.Tests.csproj
dotnet test Tests/TailwindUSS.Editor.Unit/TailwindUSS.Editor.Unit.csproj
```

variant と config を触ったフェーズでは、関連テストだけで済ませず **全体テストを必ず再実行**する。

## ドキュメント更新ルール

新しい utility family を実装したら、必ず次を更新する。

1. `README.md`
2. `docs/tailwind-uss-feature-matrix-ja.md`
3. 必要なら `docs/mvp-spec-ja.md`

## 推奨実装順の最終版

1. registry 化
2. layout / positioning / opacity
3. flex wrap / gap / self / basis / side margins
4. typography 拡張
5. border / radius / palette 拡張
6. variant 対応
7. transform / transition / cursor
8. config 拡張

この順序で進めると、**parser を壊さずに追加できる範囲**から価値を積み上げ、最後に selector / config という横断変更へ入れる。
