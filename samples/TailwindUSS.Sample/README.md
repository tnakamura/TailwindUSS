# TailwindUSS Sample Project

English | 日本語

## English

This Unity 6.3 sample project demonstrates how to use TailwindUSS from this repository through a local file dependency, with a mobile-app-style UI Toolkit sample.

### Open the sample

1. Open `samples/TailwindUSS.Sample` with Unity 6.3.
2. The sample project installs `com.tnakamura.tailwinduss` from `file:../../src/TailwindUSS`.
3. Open `Assets/UI/TailwindSample.uxml` in UI Builder, play `Assets/Scenes/SampleScene.unity`, or run `Tools/TailwindUSS/Generate` to regenerate `Assets/Generated/TailwindUSS.generated.uss` from the shell UXML plus the split screen UXML files.

### Included files

- `tailwinduss.config.json`: scans `Assets/UI/**/*.uxml` and writes `Assets/Generated/TailwindUSS.generated.uss`
- `Assets/UI/TailwindSample.uxml`: shell document that wires the app frame and instantiates the split screen UXML files
- `Assets/UI/TailwindHomeScreen.uxml`, `Assets/UI/TailwindLessonScreen.uxml`, `Assets/UI/TailwindShopScreen.uxml`, `Assets/UI/TailwindMenuScreen.uxml`: screen-specific UXML files scanned together to generate one USS
- `Assets/UI/TailwindSampleNavigation.cs`: runtime controller that switches between the Home, Lesson, Shop, and Menu screens
- `Assets/Generated/TailwindUSS.generated.uss`: generated USS kept in the repository for reference

## 日本語

この Unity 6.3 サンプルプロジェクトは、このリポジトリの TailwindUSS をローカル file 依存として利用する、モバイルアプリ風 UI の例です。

### 開き方

1. Unity 6.3 で `samples/TailwindUSS.Sample` を開きます。
2. サンプルプロジェクトは `file:../../src/TailwindUSS` から `com.tnakamura.tailwinduss` を読み込みます。
3. `Assets/UI/TailwindSample.uxml` を UI Builder で開くか、`Assets/Scenes/SampleScene.unity` を再生するか、`Tools/TailwindUSS/Generate` を実行して、シェル UXML と分割した画面 UXML 群から `Assets/Generated/TailwindUSS.generated.uss` を再生成してください。

### 含まれるファイル

- `tailwinduss.config.json`: `Assets/UI/**/*.uxml` を走査し、`Assets/Generated/TailwindUSS.generated.uss` を出力
- `Assets/UI/TailwindSample.uxml`: アプリ全体のフレームを定義し、分割した画面 UXML を組み立てるシェル UXML
- `Assets/UI/TailwindHomeScreen.uxml`, `Assets/UI/TailwindLessonScreen.uxml`, `Assets/UI/TailwindShopScreen.uxml`, `Assets/UI/TailwindMenuScreen.uxml`: 1画面ごとに分割した UXML。まとめて走査して 1 つの USS を生成する例
- `Assets/UI/TailwindSampleNavigation.cs`: ホーム / レッスン / ショップ / メニューを切り替えるランタイム制御
- `Assets/Generated/TailwindUSS.generated.uss`: 参照用にコミット済みの生成 USS
