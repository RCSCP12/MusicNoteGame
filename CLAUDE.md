# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

A Unity 6 (6000.3.9f1) educational music note reading game. Players identify musical notes displayed on a staff by pressing the corresponding letter key (A-G). Supports Treble and Bass clefs, Practice and Timed Challenge modes, and three difficulty levels.

## Development

This is a Unity project — there is no CLI build/test command. Development happens through the Unity Editor:

- Open the project in Unity Hub with Unity 6000.3.9f1
- Play from the Editor using the Play button
- Build via **File > Build Settings**
- Run in Editor starting from `Assets/Scenes/MainMenu.unity` (Scene 0)

There are no unit tests. Playtesting is done directly in the Unity Editor.

## Architecture

### Scene Flow

```
MainMenu (Scene 0) → Game (Scene 1) → [back to MainMenu or restart]
```

Player selections (clef, mode, difficulty) are passed between scenes via `PlayerPrefs`. `GameSceneInitializer` reads these on scene load and calls `GameManager.StartGame()`.

### Script Organization (`Assets/Scripts/`)

| Namespace | Key Scripts | Responsibility |
|-----------|-------------|---------------|
| `Core` | `GameManager`, `GameState`, `GameSceneInitializer` | Central game loop, state machine, scene init |
| `Gameplay` | `NoteData`, `NoteGenerator`, `LevelConfig`, `ScoreManager`, `TimerController`, `LevelManager` | Note logic, scoring, timing |
| `UI` | `MainMenuController`, `HUDController`, `GameOverPanel`, `StaffDisplay`, `NoteDisplay` | All UI rendering |
| `Audio` | `NoteAudioPlayer` | Procedural audio synthesis |
| `Input` | `NoteInputHandler` | New Input System keyboard handling |
| `Utility` | `SpriteGenerator` | Runtime sprite creation |
| `Editor` | `GameSceneSetup`, `LevelConfigCreator`, `MainMenuSceneSetup` | Editor-only tools |

### Key Design Points

**GameManager** (`Core/GameManager.cs`) is a singleton that orchestrates the entire game loop: it connects `NoteGenerator` → `NoteDisplay`/`StaffDisplay` → `NoteInputHandler` → `ScoreManager` → `HUDController`.

**LevelConfig** (`Gameplay/LevelConfig.cs`) is a ScriptableObject defining available notes per clef, time per note, and note count to win. Five assets exist in `Assets/ScriptableObjects/LevelConfigs/`; the main menu maps difficulty 0/1/2 to Level1/Level2/Level3.

**NoteAudioPlayer** (`Audio/NoteAudioPlayer.cs`) synthesizes all audio at runtime via `OnAudioFilterRead()` — no audio files exist. Correct answers play a sine wave at the note's frequency; wrong answers play a 150 Hz square wave buzz.

**NoteData** (`Gameplay/NoteData.cs`) handles staff position math: given a note name + octave + clef, it calculates the Y position on screen and whether ledger lines are needed.

**Scoring** (`Gameplay/ScoreManager.cs`): 100 base points per correct answer + 10 × streak bonus + up to 50 time bonus. High scores saved per clef+difficulty combination in PlayerPrefs as `HighScore_{ClefType}_{Difficulty}`.

### PlayerPrefs Keys

- `SelectedClef` (int) — ClefType enum value
- `SelectedMode` (int) — GameMode enum value
- `SelectedDifficulty` (int) — 0, 1, or 2
- `HighScore_{ClefType}_{Difficulty}` — int high score
