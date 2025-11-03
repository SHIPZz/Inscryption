# Repository Guidelines

## Project Structure & Module Organization
- Unity project root: this folder. Key paths:
  - `Assets/Code` — gameplay code by area: `Common`, `Infrastructure`, `Features`, `Generated`, `Editor`.
  - `Assets/Scenes` — main scenes: `Boot.unity`, `Game.unity`, `SampleScene.unity`.
  - `Assets/Configs`, `Assets/Resources`, `Assets/Prefabs`, `Assets/AddressableAssetsData` — data, assets, addressables.
  - `Packages`, `ProjectSettings`, `UserSettings` — managed by Unity.

## Build, Test, and Development Commands
- Open in Unity `6000.2.8f1` (required). Load `Assets/Scenes/Boot.unity` and press Play.
- Run Edit Mode tests (Windows, CLI):
  - `Unity.exe -batchmode -quit -projectPath . -runTests -testResults results.xml -testPlatform EditMode`
  - Use `-testPlatform PlayMode` for Play Mode tests.
- Addressables: Window → Asset Management → Addressables → Build → New Build.

## Coding Style & Naming Conventions
- C# with 4-space indent; braces on new line (Allman).
- Types/properties/methods: PascalCase. Fields: camelCase. Serialized fields: `[SerializeField] private Type fieldName;`.
- Scripts live under `Assets/Code/<Area>/<Feature>`; script files use PascalCase. Non-code assets: lowercase-with-dashes. Prefabs: PascalCase.
- Keep Entitas/Zenject pieces small and focused; prefer DI over singletons.

## Testing Guidelines
- Place tests under `Assets/Tests/EditMode` and `Assets/Tests/PlayMode`.
- File names end with `Tests.cs`; methods use `[Test]` and clear Should-style names.
- Run via Test Runner (Window → General → Test Runner) or the CLI above. Cover core systems in `Common` and gameplay `Features`.

## Commit & Pull Request Guidelines
- Commits: short, imperative tense (e.g., "refactor battle systems", "fix card stack ordering"). Group related changes.
- PRs include: summary, affected scenes/prefabs, verification steps, tests added/updated, and screenshots/GIFs for gameplay/UI.
- Link issues. Call out any ScriptableObject/config changes in `Assets/Configs`.

## Agent-Specific Notes
- Do not modify `Library`, `Temp`, `Logs`, or generated files in `Assets/Code/Generated`.
- Follow the existing folder structure and naming. New features go under `Assets/Code/Features/<FeatureName>`.
- Keep changes minimal and targeted; avoid unrelated refactors.

