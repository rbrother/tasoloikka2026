# 2D Platformer Technical Plan (Windows)

## Goal
Build a Windows desktop 2D platformer with solid movement, level content, and an extendable architecture for future features.

## Recommended Primary Stack (Best Fit)
- Language: **C#**
- Engine/Framework: **Godot 4 (Mono/.NET)**
- IDE: **JetBrains Rider** or **Visual Studio 2022**
- Art pipeline: Aseprite/Krita + Tiled (optional)
- Audio: Built-in Godot audio + external SFX/music assets
- Version control: Git (already initialized)

### Why this is the best default
- Strong C# support with fast iteration and editor tooling.
- Excellent 2D pipeline (TileMaps, collision, animation, camera, parallax, particles).
- Easier content workflow than building engine systems from scratch.
- Good balance between productivity and control for indie platformers.
- Cross-platform potential later, even if Windows is current target.

## Suggested Project Architecture (Godot + C#)
- `scenes/`
  - `MainMenu.tscn`
  - `Game.tscn`
  - `levels/Level01.tscn` ...
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
  - gameplay config in JSON or Resources

## Core Technical Decisions
- Physics: Use Godot `CharacterBody2D` for player/enemy movement.
- Input: Godot Input Map with keyboard + optional gamepad from day 1.
- Leveling: TileMap-based levels, optional Tiled import if needed.
- Save data: JSON save slots in `%APPDATA%`.
- Resolution: Start at 320x180 or 640x360 internal resolution with pixel-perfect scaling.
- Architecture style: Scene composition + small reusable C# components.

## Development Milestones
1. **Project bootstrap (1-2 days)**
   - Create Godot C# project, input mapping, base scene flow.
2. **Movement foundation (2-4 days)**
   - Run/jump/coyote time/jump buffer, camera follow, collisions.
3. **Vertical slice (1-2 weeks)**
   - One complete level, one enemy type, hazards, collectible, goal condition.
4. **Content pipeline (1 week)**
   - Level template, reusable prefabs/scenes, checkpoint system.
5. **Meta systems (1 week)**
   - UI/HUD, pause menu, settings, save/load.
6. **Polish & release prep (1-2 weeks)**
   - Juice effects, balancing, bug fixes, Windows export package.

## Possible Alternatives

### Alternative A: Unity 2D + C#
- Pros:
  - Mature ecosystem, many tutorials/assets, robust tooling.
  - Excellent C# workflow and package ecosystem.
- Cons:
  - Heavier editor/runtime overhead for small projects.
  - Licensing/ecosystem volatility concerns for some teams.
- Best when:
  - You want ecosystem breadth, Asset Store leverage, and team Unity familiarity.

### Alternative B: MonoGame + C#
- Pros:
  - Maximum control and clean C# code-centric development.
  - Lightweight runtime.
- Cons:
  - You must build many systems manually (editor/tooling, collision helpers, content workflows).
- Best when:
  - You prefer engine-level programming and custom architecture.

### Alternative C: Python + Pygame
- Pros:
  - Fast to start, very approachable.
  - Great for prototypes and learning loops/game feel.
- Cons:
  - Weaker tooling/content pipeline for larger production game.
  - Distribution/performance constraints compared to C# engines.
- Best when:
  - You want a quick prototype before committing to full production stack.

## Recommendation Summary
- **Primary recommendation:** Godot 4 + C#.
- **Fallback if you want larger ecosystem:** Unity 2D + C#.
- **Prototype-fast option:** Python + Pygame first, then port.

## Next Step (if accepted)
- Create initial Godot project skeleton in this repo and define:
  - coding conventions,
  - folder structure,
  - first playable movement scene.
