# TailwindUSS Sample Project

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
