# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**Gem TD** is a Unity tower defense game built with Universal Render Pipeline (URP). The project uses a professional architecture based on Zenject dependency injection, service-oriented design, and factory patterns.

**Unity Version:** 2022.3.x
**Product Name:** Gem TD
**Company:** sprill
**Platform:** Standalone Windows (with Android/iOS support configured)

## Game Rules

Understanding the core game mechanics is essential for working with this codebase:

### Tower Placement Phase
1. **Player places 5 towers** in locations of their choice on the map
2. Each tower spawns with:
   - **Random type** (P, Q, D, G, E, R, B, or Y)
   - **Random level** (probability weighted by player's current level)
3. **Player selects 1 tower to keep**
4. **Remaining 4 towers convert to stone obstacles**
   - Stone towers remain on the map as blocking terrain
   - They affect enemy pathfinding but provide no combat benefit

### Combat Phase
5. **Enemy wave begins** after tower selection
6. **Enemies must pass through 5 key waypoints** (control points) before reaching the finish
7. **Victory condition:** Maximize the time enemies spend traveling between waypoints and reaching the finish
   - Longer paths = more time for towers to deal damage
   - Strategic stone placement can force longer routes

### Tower Combinations
- **Special towers** can be assembled by combining 3 specific different tower types
- This mechanic allows for powerful upgraded towers through strategic planning

### Core Strategy
The game is about **pathfinding manipulation**:
- Place the 5 initial towers to create multiple path options
- Select the best tower for combat while using the other 4 as strategic obstacles
- Force enemies to take the longest possible route through all waypoints
- Balance tower placement for both combat effectiveness and path lengthening

**This explains why:**
- The `Stone` tower type exists in `TowerType` enum
- Pathfinding validation is critical after every tower placement
- The `MapData.Points` list defines the 5 waypoints enemies must traverse
- Level randomization depends on player level (currently returning 0)

## Build Commands

### Opening the Project
```bash
# Open project in Unity Editor (requires Unity Hub)
# The project uses Unity 2022.3.x
# Install Unity Hub and open: C:\UnityProjects\gem_td
```

### Building the Game
The project has two scenes configured for builds:
1. `Assets/Scenes/Boot.unity` (Bootstrap scene with project-wide DI setup)
2. `Assets/Scenes/Game.unity` (Main game scene)

**Build via Unity Editor:**
- File → Build Settings → Build (or Build and Run)
- Builds output to `Builds/` directory

**No automated build scripts are currently configured.**

### Running Tests
No test framework is currently set up in the project. Unity Test Framework package is included but no test assemblies exist.

## Architecture Overview

### Dependency Injection (Zenject)

The project uses a **two-installer pattern** for dependency injection:

1. **BootstrapInstaller** (`Assets/Scenes/Boot.unity`):
   - Project-wide singletons that persist across scenes
   - Binds: `IUpdateService`, `IResourceProvider`
   - Attached to a GameObject in Boot scene

2. **SceneInstaller** (`Assets/Scenes/Game.unity`):
   - Scene-specific singletons
   - Binds: `IInputService`, `ITowerFactory`, `ITowerService`, `IBlockFactory`, `ILevelBuilder`, `InputActions`, `IPathService`, `IPathDrawer`
   - Attached to a GameObject in Game scene

**Key Pattern:** All services use constructor injection with `[Inject]` attribute. Factories use `DiContainer.InstantiatePrefabForComponent<T>()` to ensure DI works on prefab instances.

### Core Systems

#### 1. Input System
- **Files:** `Assets/Scripts/infrastructure/services/inputService/`
- Uses Unity's new Input System (com.unity.inputsystem)
- `InputActions.cs` is auto-generated from `Assets/Input/InputActions.inputactions` (not shown in scripts)
- `InputService` translates input actions into game events:
  - Mouse left click → raycast for `IClickable` objects (0.5s duration threshold)
  - Space → builds the level map
  - P → toggles pathfinding visualization

**To regenerate InputActions.cs:**
1. Select `Assets/Input/InputActions.inputactions` in Unity Editor
2. Check "Generate C# Class" in Inspector
3. Click "Apply"

#### 2. Level Building System
- **Files:** `Assets/Scripts/level/builder/`
- `MapData` (ScriptableObject) defines level layout as 2D grid of `BlockType` enums
- Uses Odin Inspector for custom editor visualization (painting blocks)
- `LevelBuilder` orchestrates:
  1. Loads MapData from Resources
  2. Instantiates blocks via `BlockFactory`
  3. Manages tower placement with validation
  4. Calls `PathService` to validate enemy paths after tower placement

**Block Types:**
- `None`, `Dark`, `Light` (walkable ground)
- `Point` (waypoint), `Way` (path visual)
- `NoPut` (tower-restricted), `Start`, `End` (path endpoints)

#### 3. Pathfinding (A* Algorithm)
- **Files:** `Assets/Scripts/infrastructure/services/pathService/`, `Assets/Scripts/level/path/`
- `PathService` implements A* with:
  - Manhattan distance heuristic
  - 8-way movement (orthogonal cost: 10, diagonal: 16)
  - Diagonal squeeze-through prevention (can't pass between two blocked adjacent cells)
  - Multi-waypoint chaining (follows `MapData.Points` in sequence)
- Returns `List<Vector2Int>` of grid positions
- Fires `OnPathChange` event when path updates
- `PathDrawer` visualizes path with LineRenderer (toggle with P key)

**Waypoint System:**
- `MapData.Points` defines the **5 control points** enemies must traverse
- Path is chained: Start → Point 1 → Point 2 → Point 3 → Point 4 → Point 5 → End
- This is the core mechanic: longer paths between waypoints = more tower damage time

**Critical:** Always validate paths after tower placement to prevent soft-locking (enemies can't reach end).

#### 4. Tower System
- **Files:** `Assets/Scripts/towers/`
- `TowerData` (ScriptableObject) defines tower stats (Damage, AttackSpeed, AttackRange)
- Located in `Resources/ScriptableObjects/Towers/`
- Tower placement flow:
  1. User clicks `Block` → `Block.OnClick()`
  2. `LevelBuilder.CreateTower(x, y)` validates placement
  3. `TowerService.GetTowerTypeFromChance()` randomizes type (2-9: P, Q, D, G, E, R, B, Y)
  4. `TowerService.GetLevelFromChance()` randomizes level based on player level
  5. `PathService.FindPath()` validates enemy routes still exist
  6. If valid, `TowerFactory.CreateTower(type, level)` instantiates
  7. `TowerFactory.CreateTowerModel()` adds visual model with level-based scaling

**Tower Types:**
- Enum values 0-9: `None`, `Stone`, `P`, `Q`, `D`, `G`, `E`, `R`, `B`, `Y`
- `Stone` (1): Special type for converting unselected towers into obstacles
- `P-Y` (2-9): Combat towers with 8 different types
- Legacy comments show planned 6-level expansion per type (not implemented)

**Tower Lifecycle:**
1. **Placement:** Player places 5 random towers
2. **Selection:** Player chooses 1 tower to keep
3. **Conversion:** Remaining 4 towers convert to `TowerType.Stone` obstacles
4. **Combat:** Selected tower engages enemies during wave

**Incomplete:**
- Tower attacking/targeting logic is scaffolded but not functional
- Tower selection UI/mechanic not implemented
- Stone conversion logic not implemented
- `GetLevelFromChance()` currently always returns 0 (needs player level integration)

#### 5. Enemy System
- **Files:** `Assets/Scripts/enemies/`
- `EnemyData` (ScriptableObject) defines stats (Health, MoveSpeed, RotateSpeed, Damage, Armor, MagicResist, Evasion)
- `Enemy` subscribes to `PathService.CurrentPath` via constructor injection
- Movement logic:
  1. Rotate toward next waypoint
  2. Move forward at `MoveSpeed`
  3. Advance to next waypoint when reached
- Converts `Vector2Int` path to `Vector3` world positions

**Incomplete:** Combat, damage, and special movement types not implemented.

#### 6. Factory Pattern
All factories follow this pattern:
- Constructor loads prefabs/data from Resources
- `Create()` methods use `DiContainer.InstantiatePrefabForComponent<T>()` for DI support
- Assets are cached in private dictionaries for performance

**Factories:**
- `TowerFactory`: Creates towers and tower models (visual prefabs)
- `BlockFactory`: Creates block GameObjects
- `EnemyFactory`: Interface exists but not implemented

### Resource Loading

**Convention:** All game assets load via `Resources.Load()` from these paths:
- Maps: `Resources/ScriptableObjects/Maps/Map`
- Towers: `Resources/ScriptableObjects/Towers/` (multiple files)
- Block Prefabs: `Resources/Prefabs/Blocks/`
- Tower Prefabs: `Resources/Prefabs/Towers/`
- Path Visuals: `Resources/Prefabs/Path/Line`

**Service:** `IResourceProvider` wraps `Resources.Load<T>()` for testability.

## Code Conventions

### Naming
- **Interfaces:** `I` prefix (`IInputService`, `ITowerFactory`)
- **Private fields:** `_camelCase` with underscore prefix
- **Data classes:** `*Data` suffix (`TowerData`, `EnemyData`)
- **Settings:** `*Settings` suffix (`TowerSettings`)
- **Enums:** PascalCase (`BlockType`, `TowerType`)
- **Namespaces:** Match folder structure (e.g., `infrastructure.services.inputService`)

### Dependency Injection
```csharp
// Constructor injection pattern
public class MyService : IMyService
{
    private readonly IOtherService _otherService;

    [Inject]
    public MyService(IOtherService otherService)
    {
        _otherService = otherService;
    }
}

// Installer binding
Container.Bind<IMyService>().To<MyService>().AsSingle();

// Instantiating prefabs with DI
Container.InstantiatePrefabForComponent<Tower>(prefab);
```

### ScriptableObject Data Pattern
Configuration lives in ScriptableObjects, not code:
```csharp
[CreateAssetMenu(fileName = "NewData", menuName = "Game/Data")]
public class MyData : ScriptableObject
{
    public int SomeValue;
    public float SomeOther;
}
```

### Event-Driven Communication
Systems communicate via events to avoid tight coupling:
```csharp
public event Action<Vector2> OnSomeEvent;

// Fire event
OnSomeEvent?.Invoke(data);

// Subscribe
service.OnSomeEvent += HandleEvent;
```

## Common Workflows

### Adding a New Service

1. Create interface in `Assets/Scripts/infrastructure/services/myService/IMyService.cs`
2. Implement in `MyService.cs` with `[Inject]` constructor
3. Add binding in `SceneInstaller.cs` or `BootstrapInstaller.cs`:
   ```csharp
   Container.Bind<IMyService>().To<MyService>().AsSingle();
   ```
4. Use `.NonLazy()` if service should initialize immediately

### Adding a New Tower Type

1. Add enum value to `TowerType.cs`
2. Create `TowerData` asset in `Resources/ScriptableObjects/Towers/`
3. Create visual prefab (TowerModel) in `Resources/Prefabs/Towers/`
4. Ensure prefab has `TowerModel` component with matching `TowerType`
5. Update `TowerService.GetTowerTypeFromChance()` if needed

### Adding a New Block Type

1. Add enum value to `BlockType.cs`
2. Create prefab in `Resources/Prefabs/Blocks/`
3. Ensure prefab has `Block` component
4. Update `LevelBuilder.CanSetOnBlock()` if placement rules change

### Modifying the Map

1. Open `Resources/ScriptableObjects/Maps/Map` in Inspector
2. Use Odin Inspector's custom editor to paint blocks
3. Set waypoints in `Points` list (order matters: Start → Point 1-5 → End)
4. Ensure exactly 5 control points are defined (per game rules)
5. Map dimensions are `Width` × `Height` with `BlockSize` world units

### Implementing Tower Combinations (Future)

Special towers are created by combining 3 different tower types. When implementing this system:

1. **Define Combination Recipes:**
   - Create a `TowerCombination` ScriptableObject or data structure
   - Define which 3 `TowerType` values combine into a special tower
   - Example: `P + Q + D = SpecialTowerX`

2. **Detection Logic:**
   - Check adjacent towers when placing a new tower
   - Use pathfinding grid positions to detect 3-tower groups
   - Validate all 3 towers are different types

3. **Combination System:**
   - Add new `TowerType` enum values for special towers (10+)
   - Create corresponding `TowerData` and visual models
   - Implement transformation: remove 3 source towers, spawn 1 special tower

4. **Integration Points:**
   - `TowerService`: Add `CheckCombinations(x, y)` method
   - `LevelBuilder`: Call after tower placement
   - `TowerFactory`: Support special tower creation

## Important Implementation Details

### Path Validation is Critical
Always call `PathService.FindPath()` after modifying `_towerMap` to ensure enemies can still reach the end. If path is null, revert the change:
```csharp
_towerMap[x, y] = TowerType.SomeTower;
var path = _pathService.FindPath(_mapData, _towerMap);
if (path == null) {
    _towerMap[x, y] = TowerType.None; // Revert
    return null;
}
```

### Diagonal Movement Rules
The A* implementation prevents diagonal "squeezing" through two diagonally adjacent blocked cells. This prevents enemies from cutting corners unrealistically.

### Update Service Pattern
`UpdateService` is a MonoBehaviour that broadcasts Unity's Update/FixedUpdate to services:
```csharp
public interface IUpdateService
{
    event Action OnUpdate;
    event Action OnFixedUpdate;
}
```
Subscribe to this instead of making every service a MonoBehaviour.

### Factory Asset Loading
Factories load assets in constructors using `Resources.LoadAll<T>()` and cache in dictionaries. This happens during DI initialization. If assets are missing, factories will fail silently or return null—check Resources paths carefully.

### Odin Inspector Dependency
The project uses Sirenix Odin Inspector (commercial Unity plugin) for:
- `MapData` custom editor (grid painting interface)
- Enhanced Inspector features throughout

**Note:** Odin Inspector is not required for runtime, only for Unity Editor tooling.

## Known Incomplete Features

These systems are partially implemented and require completion to match the game rules:

1. **Tower Placement & Selection Phase:**
   - ❌ No "place 5 towers at once" mechanic implemented
   - ❌ No UI for selecting 1 tower to keep
   - ❌ No logic to convert unselected towers to `TowerType.Stone`
   - ❌ No state machine for placement → selection → combat phases
   - **Current:** Player clicks to place one tower at a time

2. **Player Level System:**
   - ❌ No player level tracking
   - ❌ `TowerService.GetLevelFromChance()` always returns 0 (should be weighted by player level)
   - ❌ Tower level probability distribution not implemented
   - **Required:** Player level must affect tower level randomization

3. **Tower Combat:**
   - ✅ Tower tracks enemies in range via `EnemyTrigger` collider events (done)
   - ❌ `TowerData.Damage`, `AttackSpeed`, `AttackRange` defined but unused
   - ❌ No attack/targeting logic implemented
   - ❌ `Spell.cs` exists but never instantiated
   - **Required:** Towers must damage enemies during combat phase

4. **Tower Combinations:**
   - ❌ No system for combining 3 different towers into special towers
   - ❌ No special tower definitions or combination recipes
   - **Required:** Implement combination detection and special tower creation

5. **Enemy Combat:**
   - ✅ Enemy pathfinding through waypoints works (done)
   - ❌ `EnemyData` has Armor, MagicResist, Evasion fields (unused)
   - ❌ No damage/health system
   - ❌ No death/wave completion logic
   - ❌ `EnemyMoveType` enum exists but not utilized

6. **Wave System:**
   - ❌ No wave spawning after tower selection
   - ❌ No wave progression or difficulty scaling
   - ❌ No victory/defeat conditions
   - **Required:** Trigger enemy wave after player selects tower to keep

7. **Services:**
   - `ILevelService`/`LevelService` are empty (no DI binding)
   - `IEnemyFactory` is empty interface (not bound in installer)

8. **UI System:**
   - `UiManager`, `MenuView`, `PlayView`, `SettingsView`, `Popup` exist in `Assets/Scripts/ui/`
   - Integration with game flow unclear (not analyzed in this document)
   - **Required:** Tower selection UI, player level display, wave progress

## Third-Party Packages

Key dependencies (see `Packages/manifest.json`):
- **Zenject** (Extenject): Dependency injection framework (installed as asset)
- **Unity Input System** (1.14.0): New input handling
- **Cinemachine** (2.10.3): Camera system
- **Universal Render Pipeline** (14.0.12): Rendering
- **TextMeshPro** (3.0.7): UI text rendering
- **Odin Inspector**: Enhanced Unity Editor (commercial, in Assets/Plugins/Sirenix)
- **DOTween**: Tweening library (installed as asset)
- **UniTask**: Async/await utilities (installed as asset)
- **Toony Colors Pro 2**: Shader framework (in Assets/Plugins)

## Git Workflow

No specific git conventions documented. Current branch: `main`.

Recent commits show active development on:
- Tower package integration
- A* pathfinding completion
- Map builder and tower creation

## File Structure Summary

```
Assets/
├── Input/                  # InputActions (generated)
├── Scenes/                 # Boot.unity, Game.unity
├── Scripts/
│   ├── cameras/           # CameraController
│   ├── enemies/           # Enemy, EnemyData, EnemyMoveType
│   ├── infrastructure/
│   │   ├── factories/    # TowerFactory, BlockFactory, EnemyFactory
│   │   ├── installers/   # BootstrapInstaller, SceneInstaller
│   │   └── services/     # InputService, PathService, TowerService, etc.
│   ├── level/
│   │   ├── builder/      # LevelBuilder, MapData, Block
│   │   └── path/         # PathDrawer, IPathDrawer
│   ├── towers/           # Tower, TowerData, TowerModel, Spell
│   └── ui/               # UiManager, MenuView, PlayView, SettingsView, Popup
├── Plugins/              # Zenject, Odin Inspector, DOTween, UniTask, TCP2
└── Resources/
    ├── Prefabs/          # Blocks/, Towers/, Path/
    └── ScriptableObjects/ # Maps/, Towers/, Enemies/
```

## Game Phase Flow (Target Architecture)

The game should follow this state machine when fully implemented:

### Phase 1: Tower Placement
```
State: PLACING_TOWERS
Input: Mouse clicks on valid blocks
Output: 5 random towers placed on map
```
1. Player clicks block
2. System validates placement (path must remain valid)
3. `TowerService.GetTowerTypeFromChance()` generates random type (P-Y)
4. `TowerService.GetLevelFromChance(playerLevel)` generates random level
5. `TowerFactory.CreateTower(type, level)` instantiates tower
6. Repeat until 5 towers placed
7. **Transition to Phase 2**

### Phase 2: Tower Selection
```
State: SELECTING_TOWER
Input: Click on one of the 5 placed towers
Output: 1 combat tower, 4 stone obstacles
```
1. UI highlights the 5 placed towers
2. Player clicks one tower to keep
3. Selected tower remains as combat tower
4. Other 4 towers convert to `TowerType.Stone` (obstacles only, no combat)
5. Path recalculates around all obstacles
6. **Transition to Phase 3**

### Phase 3: Enemy Wave
```
State: COMBAT
Input: Time progression
Output: Enemy spawns, movement, combat, wave completion
```
1. Enemy wave spawns at Start point
2. Enemies follow path: Start → Point 1 → Point 2 → Point 3 → Point 4 → Point 5 → End
3. Towers attack enemies in range
4. Track enemy health, apply damage
5. On wave clear: award resources/XP, increase player level
6. **Loop back to Phase 1** or proceed to next level

### Phase 4: Victory/Defeat (Future)
```
State: GAME_OVER
Trigger: All waves cleared OR enemies reach end with lives = 0
```

### Current Implementation Status
- ✅ **Phase 1:** Partially working (one tower at a time, no 5-tower limit)
- ❌ **Phase 2:** Not implemented
- ❌ **Phase 3:** Enemy movement works, but no combat/waves/spawning
- ❌ **Phase 4:** Not implemented

### Required Services
- **GameStateService** (not yet created): Manages phase transitions
- **WaveService** (not yet created): Spawns enemies, tracks wave progress
- **PlayerService** (not yet created): Tracks player level, resources, lives

## Important Design Patterns

- **Dependency Injection:** Zenject-based container pattern for loose coupling
- **Factory Pattern:** Encapsulates complex object creation (towers, blocks)
- **Observer/Events:** Decoupled cross-system communication
- **Service Locator:** DI container provides global service access
- **A* Algorithm:** Pathfinding with heuristic optimization
- **ScriptableObject Configuration:** Data-driven design for game entities
- **Interface Segregation:** Minimal, focused contracts (IClickable, IPathService)

## Performance Considerations

- **No Object Pooling:** Towers/enemies are instantiated fresh (potential GC pressure at scale)
- **Physics-Based Detection:** `EnemyTrigger` uses colliders for range detection (could impact performance with many towers)
- **Resource Caching:** Factories cache prefabs/data in dictionaries after initial load
- **A* Optimization:** Uses closed list to avoid revisiting nodes, but no pathfinding cache for repeated queries

## Scene Setup

The game requires both scenes to function:
1. **Boot.unity** must load first (sets up project context)
2. **Game.unity** loads next (sets up scene context and gameplay)

This is configured in `ProjectSettings/EditorBuildSettings.asset`.

**use ai-game-developer mcp to interact with Unity**