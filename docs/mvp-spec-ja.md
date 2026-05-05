# TailwindUSS MVP仕様書

## 位置づけ

この文書は、`implementation-approaches-ja.md` で推奨した方針をもとに、TailwindUSS の最初の実装対象を MVP として具体化した仕様書である。

MVP では、**Unity UI Toolkit 向けに、Tailwind CSS ライクなユーティリティ記法を UXML から読み取り、USS を自動生成する Unity Editor 拡張**を提供する。

---

## MVPの目的

- Unity 上で Tailwind ライクな記法が成立することを検証する
- UI Toolkit / USS に対して実用的な最小ユーティリティ集合を提供する
- Unity Editor 内だけで完結する初期体験を実現する
- 将来の拡張に耐えられる最小構成を定める

---

## MVPで採用する方針

1. **Unity UI Toolkit 専用**
2. **Tailwind 完全互換は目指さず、Unity 向けサブセットを定義**
3. **入力は UXML の `class` 属性を基本とする**
4. **C# ネイティブでクラス文字列を解析する**
5. **使用されたユーティリティだけを静的スキャンして USS を生成する**
6. **生成タイミングは明示コマンドを基本とする**

---

## MVPで提供する価値

- Web の Tailwind に似た短いユーティリティ記法で UI Toolkit を記述できる
- 手で USS を書く量を減らせる
- UI ごとに必要なスタイルだけを自動生成できる
- Unity 導入時に Node.js を必須にしない

---

## 対象範囲

### 対象

- Unity Editor 拡張
- UI Toolkit
- UXML ファイル
- USS 自動生成
- 最小限の設定ファイル

### 非対象

- uGUI
- Tailwind CLI 連携
- Node.js 依存
- 完全な Tailwind 互換
- 完全な CSS 互換
- レスポンシブ対応
- arbitrary value の本格対応
- UI Builder 深い統合
- Runtime 時の動的生成

---

## 想定利用シナリオ

利用者は UXML 上で以下のようにクラスを記述する。

```xml
<ui:VisualElement class="flex flex-row items-center px-4 py-2 bg-blue-500 rounded" />
<ui:Label class="text-sm text-white" text="Play" />
```

その後、Unity Editor メニューから TailwindUSS の生成コマンドを実行する。

生成処理はプロジェクト内の対象 UXML を走査し、使われているユーティリティクラスだけを集計し、対応する USS を出力する。

---

## ユーザー体験

### 初期導入

1. パッケージを導入する
2. TailwindUSS 設定ファイルを生成する
3. UXML にユーティリティクラスを書く
4. `Tools/TailwindUSS/Generate` を実行する
5. 生成された USS を UXML から参照する

### 日常運用

- UXML を編集する
- 必要に応じて再生成する
- 未対応クラスや変換失敗は Editor 上で確認する

---

## 入力仕様

### 入力媒体

MVP で正式対応する入力媒体は **UXML の `class` 属性** のみとする。

例:

```xml
<ui:Button class="px-4 py-2 bg-blue-500 text-white rounded" />
```

### クラス文字列の解釈

- 半角スペース区切りでトークン化する
- 順不同で指定可能
- 同一トークンの重複は 1 回として扱う
- 未対応トークンは警告対象とする

### 通常クラスとの共存

MVP では、既存 USS 用クラスと TailwindUSS 用ユーティリティを同じ `class` 属性内に混在可能とする。

例:

```xml
<ui:Button class="primary-button px-4 py-2 rounded" />
```

ただし、TailwindUSS 側は「サポート対象ユーティリティとして認識できたトークン」のみを生成対象とする。

---

## 対応ユーティリティ範囲

MVP では、以下のカテゴリに限定する。

### 1. レイアウト

- `flex`
- `hidden`
- `flex-row`
- `flex-col`
- `grow`
- `shrink`

### 2. 配置

- `items-start`
- `items-center`
- `items-end`
- `justify-start`
- `justify-center`
- `justify-end`
- `justify-between`

### 3. 余白

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

`*` は MVP の spacing scale に含まれる値のみ対応する。

### 4. サイズ

- `w-*`
- `h-*`
- `min-w-*`
- `min-h-*`
- `max-w-*`
- `max-h-*`

### 5. 色

- `bg-*`
- `text-*`
- `border-*`

### 6. タイポグラフィ

- `text-xs`
- `text-sm`
- `text-base`
- `text-lg`
- `font-normal`
- `font-bold`

### 7. ボーダー

- `border`
- `border-0`
- `border-2`

### 8. 角丸

- `rounded-none`
- `rounded-sm`
- `rounded`
- `rounded-md`
- `rounded-lg`
- `rounded-full`

---

## トークンスケール

### spacing scale

MVP では次の spacing scale を持つ。

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

### color scale

MVP では最小限の固定パレットを持つ。

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

### font size scale

| token | value |
| --- | --- |
| xs | 12px |
| sm | 14px |
| base | 16px |
| lg | 18px |

---

## 出力仕様

### 出力形式

- Unity の USS ファイルを生成する
- ファイル名はデフォルトで `TailwindUSS.generated.uss` とする
- 生成先は設定で変更可能にする

### 生成単位

MVP では **プロジェクト全体で 1 つの生成 USS** を出力する。

理由:

- 実装が単純
- 再生成と参照管理が簡単
- MVP 段階で運用を理解しやすい

### クラス生成ルール

- 1 ユーティリティにつき 1 つの USS クラスを生成する
- クラス名は人間可読な形式を採用する
- MVP では入力トークンをそのまま USS クラス名として使う

例:

- `px-4` → `.px-4`
- `bg-blue-500` → `.bg-blue-500`
- `text-sm` → `.text-sm`

### UXML との対応

MVP では、生成 USS のクラス名は入力トークンと一致させる。

このため、UXML 側のクラス文字列は生成時に書き換えない。

将来的に命名衝突やエスケープの問題が大きくなった場合に備えて、内部実装では `tw-` プレフィックス方式やハッシュ方式へ移行しやすい構造にする。

---

## 生成処理仕様

生成コマンドは次の流れで動作する。

1. 設定を読み込む
2. 対象 UXML を列挙する
3. `class` 属性を抽出する
4. トークンを分解する
5. サポート対象ユーティリティだけを抽出する
6. ユーティリティごとに USS ルールへ変換する
7. 重複排除して 1 つの USS ファイルに出力する
8. 結果と警告を Unity Console に表示する

---

## 設定仕様

MVP ではテキストベースの設定ファイルを採用する。

ファイル名候補:

- `tailwinduss.config.json`

MVP で持つ項目:

```json
{
  "inputGlobs": ["Assets/**/*.uxml"],
  "outputUssPath": "Assets/Generated/TailwindUSS.generated.uss",
  "autoAttachGeneratedUss": false
}
```

### 各項目

- `inputGlobs`: スキャン対象 UXML
- `outputUssPath`: 生成する USS の出力先
- `autoAttachGeneratedUss`: 生成 USS の自動参照付与を行うか

---

## Unity Editor 機能仕様

MVP では以下を提供する。

- `Tools/TailwindUSS/Generate`
- `Tools/TailwindUSS/Validate`
- `Tools/TailwindUSS/Create Default Config`

### Generate

- USS を生成する
- 成功件数、警告件数、失敗件数を表示する

### Validate

- UXML を走査し、未対応トークンを一覧表示する
- ファイル名と要素位置が分かる範囲で出す

### Create Default Config

- デフォルト設定ファイルを生成する

---

## エラー・警告仕様

### 警告にするもの

- 未対応ユーティリティ
- サポート対象だが値が不正なトークン
- 重複トークン

### エラーにするもの

- 設定ファイルが壊れている
- 出力先ディレクトリが作成できない
- USS ファイル書き込みに失敗する

### 表示方針

- Unity Console に出力する
- 可能な限り対象ファイルを含める
- 変換不能な 1 トークンで全体を止めず、継続可能なものは継続する

---

## 非機能要件

### パフォーマンス

- 中規模プロジェクトで数百ファイルの UXML を処理できること
- 同一トークンの重複生成を避けること

### 保守性

- パーサー、トークン解決、USS 出力を分離する
- ユーティリティ定義はテーブル化する

### 拡張性

- 将来、独自 `tw` 属性を追加できる構造にする
- 将来、ScriptableObject 設定に差し替え可能にする
- 将来、バリアントや arbitrary value の限定対応を追加しやすくする

---

## 内部モジュール構成案

MVP では最低限、以下の責務分割を推奨する。

- `ConfigLoader`
- `UxmlScanner`
- `ClassTokenParser`
- `UtilityResolver`
- `UssEmitter`
- `GenerationService`
- `ValidationService`

---

## 受け入れ条件

以下を満たしたら MVP 完了とする。

1. UXML の `class` 属性からユーティリティトークンを検出できる
2. サポート対象カテゴリの USS を自動生成できる
3. 未対応トークンを警告表示できる
4. 設定ファイルの生成と読込ができる
5. Unity Editor メニューから生成操作できる
6. サンプル UXML で実際に見た目が反映される

---

## MVP完了時点で明示する制約

- Web 版 Tailwind と完全一致しない
- レスポンシブは未対応
- 疑似クラスは未対応
- arbitrary value は未対応
- C# コード上の `AddToClassList` 解析は未対応
- UI Builder 独自補完は未対応

---

## MVP後の優先拡張候補

1. 自動再生成
2. 独自 `tw` 属性
3. 疑似クラスの限定対応
4. C# API 対応
5. ScriptableObject 設定
6. UI Builder 連携
7. arbitrary value の限定対応

---

## まとめ

TailwindUSS の MVP は、**Unity UI Toolkit に対して、UXML の `class` 属性から Tailwind ライクなユーティリティを静的抽出し、1 つの USS を生成する Editor 拡張**として定義する。

この仕様により、最小実装で価値検証を行いつつ、将来的な自動化、設定強化、対応範囲拡大へ自然に発展できる。
