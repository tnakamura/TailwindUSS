# TailwindUSS Sample Project

English | 日本語

## English

This Unity 6.3 sample project demonstrates how to use TailwindUSS from this repository through a local file dependency.

### Open the sample

1. Open `samples/TailwindUSS.Sample` with Unity 6.3.
2. The sample project installs `com.tnakamura.tailwinduss` from `file:../../src/TailwindUSS`.
3. Open `Assets/UI/TailwindSample.uxml` in UI Builder, or run `Tools/TailwindUSS/Generate` to regenerate `Assets/Generated/TailwindUSS.generated.uss`.

### Included files

- `tailwinduss.config.json`: scans `Assets/UI/**/*.uxml` and writes `Assets/Generated/TailwindUSS.generated.uss`
- `Assets/UI/TailwindSample.uxml`: sample UI Toolkit document styled with TailwindUSS utility classes
- `Assets/Generated/TailwindUSS.generated.uss`: generated USS kept in the repository for reference

## 日本語

この Unity 6.3 サンプルプロジェクトは、このリポジトリの TailwindUSS をローカル file 依存として利用する例です。

### 開き方

1. Unity 6.3 で `samples/TailwindUSS.Sample` を開きます。
2. サンプルプロジェクトは `file:../../src/TailwindUSS` から `com.tnakamura.tailwinduss` を読み込みます。
3. `Assets/UI/TailwindSample.uxml` を UI Builder で開くか、`Tools/TailwindUSS/Generate` を実行して `Assets/Generated/TailwindUSS.generated.uss` を再生成してください。

### 含まれるファイル

- `tailwinduss.config.json`: `Assets/UI/**/*.uxml` を走査し、`Assets/Generated/TailwindUSS.generated.uss` を出力
- `Assets/UI/TailwindSample.uxml`: TailwindUSS の utility class でスタイルした UI Toolkit ドキュメント
- `Assets/Generated/TailwindUSS.generated.uss`: 参照用にコミット済みの生成 USS
