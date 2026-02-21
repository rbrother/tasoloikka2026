# Tasoloikka2026 - Godot 4 + C# Scaffold

This repository now contains a starter framework for a 2D platformer on Windows.

## Included
- Godot project config (`project.godot`)
- Scene layout (`scenes/`)
- Basic level with ground (`scenes/levels/Level01.tscn`)
- Playable character controller with:
  - horizontal movement
  - jump
  - coyote time
  - jump buffering
- Basic HUD/debug text

## Open and run
1. Open the folder in Godot 4 (Mono/.NET build).
2. If prompted, generate the C# solution/project from the editor.
3. Run the main scene (`scenes/Game.tscn`).

## Current controls
- Move: `Left/Right` or `A/D` (Godot `ui_left` / `ui_right`)
- Jump: `Space` / `Enter` (`ui_accept`)

## Recommended next implementation steps
1. Add custom input actions: `move_left`, `move_right`, `jump`.
2. Replace placeholder geometry with sprite + animation.
3. Add death zones, checkpoints, and level restart flow.
4. Add enemy scene + simple patrol AI.
