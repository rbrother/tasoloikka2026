# 2D Platformer Technical Plan (Windows)

## Goal
Build a Windows desktop 2D platformer with solid movement, level content, and an extendable architecture for future features.

## Selected Stack
- Language: **C#**
- Engine/Framework: **Godot 4 (Mono/.NET)**
- IDE: **JetBrains Rider** or **Visual Studio 2022**
- Art pipeline: Aseprite/Krita + Tiled (optional)
- Audio: Built-in Godot audio + external SFX/music assets
- Version control: Git

## Why This Stack
- Strong C# support with fast iteration and editor tooling.
- Excellent 2D workflow (TileMaps, collision, animation, camera, parallax, particles).
- Efficient content pipeline for platformers.
- Good balance between productivity and technical control.

## Current Status
- Godot project opens successfully.
- Basic playable prototype exists:
  - yellow player block
  - green ground
  - movement + jump baseline

## Suggested Project Architecture
- `scenes/`
  - `MainMenu.tscn`
  - `Game.tscn`
  - `levels/Level01.tscn`
  - `player/Player.tscn`
  - `enemies/`
  - `ui/HUD.tscn`
- `scripts/`
  - `player/PlayerController.cs`
  - `core/GameManager.cs`
  - `core/InputRouter.cs`
  - `systems/SaveSystem.cs`
  - `systems/AudioSystem.cs`
- `assets/`
  - `sprites/`, `tilesets/`, `audio/`
- `data/`
  - gameplay config JSON/resources

## Core Technical Decisions
- Physics: Use `CharacterBody2D` for player/enemy movement.
- Input: Godot Input Map with keyboard + optional gamepad from day 1.
- Levels: TileMap-based level construction.
- Save data: JSON save slots in `%APPDATA%`.
- Resolution: 640x360 internal resolution with pixel-perfect scaling.
- Architecture: Scene composition + small reusable C# components.

## Development Milestones
1. **Foundation polish (2-4 days)**
   - Finalize input actions (`move_left`, `move_right`, `jump`), tune movement/jump feel.
2. **Vertical slice (1-2 weeks)**
   - One complete level, one enemy type, hazards, collectible, level finish flow.
3. **Content pipeline (1 week)**
   - Reusable level template, checkpoints, scene/prefab conventions.
4. **Meta systems (1 week)**
   - HUD, pause menu, settings, save/load.
5. **Polish & release prep (1-2 weeks)**
   - Effects, balancing, bug fixes, Windows export package.

## Immediate Next Steps
1. Add custom input actions and switch scripts to them.
2. Replace placeholder blocks with sprite + animation setup.
3. Add checkpoint/death/restart gameplay loop.
4. Add first enemy scene with simple patrol AI.
