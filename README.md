# Runshift

A first-person 3D grappling hook game built with **Godot 4** and **C# (.NET)**.

## Overview

Runshift is a first-person movement game centered around a grappling hook mechanic. Aim at any surface tagged as `Hookable`, fire the hook with a left click, and slingshot yourself across the level.

## Features

- **Grappling Hook** – Left-click to fire a raycast-based grapple. Click again to release.
- **First-Person Movement** – WASD movement with mouse-look camera control.
- **Sprint** – Hold sprint to move faster.
- **Jump** – Standard jump while grounded.
- **Debug Mode** – Optional debug logging and visual hit markers for the grapple raycast.

## Controls

| Action | Input |
|---|---|
| Move | WASD |
| Look | Mouse |
| Jump | Space |
| Sprint | Shift |
| Grapple / Release | Left Click |
| Pause | Escape |

## Project Structure

```
grapling-project/
├── main/           # Main game scene and script
├── player/         # Player character (CharacterBody3D + grapple logic)
├── MainMenu/       # Main menu scene and script
├── PauseMenu/      # Pause menu scene and script
├── LevelGui/       # HUD / level GUI
├── Crosshair/      # Crosshair overlay
└── TestScenes/     # Sandbox / test scenes
```

## Requirements

- [Godot 4.6+](https://godotengine.org/) with **.NET / C#** support
- [.NET SDK 8+](https://dotnet.microsoft.com/download)

## Getting Started

1. Clone the repository:
   ```bash
   git clone <repo-url>
   cd grapling-project
   ```
2. Open the project in Godot 4 by selecting the `project.godot` file.
3. Build the C# solution (**Build → Build Solution** or press `Alt+B`).
4. Press **F5** (or the Play button) to run the game.

## Development Notes

- Surfaces must belong to the **`Hookable`** group to be grapple-able.
- Player properties (speed, grapple speed, sensitivity, etc.) are exposed as `[Export]` variables and can be tweaked directly in the Godot Inspector.
- Enable the `Debugging` export on the Player node for verbose raycast output and green sphere hit markers.

