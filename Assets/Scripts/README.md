# Kenney Jam 25 - Script Organization

This document describes the organization and structure of the Unity project scripts for the Surreal Sled Adventure game.

## Project Structure

The scripts are organized into the following directories:

### 📁 Core/
**Core game systems and managers**
- `GameManager.cs` - Main game state management, score, lives, checkpoints
- `PuzzleGameManager.cs` - Comprehensive puzzle system management and progression
- `SaveSystem.cs` - Save/load game data functionality with multiple save slots
- `SaveData.cs` - Data structures for save system
- `ExperienceSystem.cs` - Player experience and leveling system
- `StoryRouteManager.cs` - Story progression and choice management
- `StorySequencer.cs` - Advanced story sequence control
- `YAMLContentLoader.cs` - YAML content loading for stories and dialogue
- `GameSetup.cs` - Initial game configuration and setup

### 📁 Player/
**Player-related scripts**
- `PlayerController.cs` - Main player movement and controls
- `FirstPersonController.cs` - Advanced 3D first-person movement with physics
- `InputModeController.cs` - FPV/UI mode switching and cursor management
- `PlayerInput.cs` - Input handling for player actions and cross-platform support
- `PlayerInventory.cs` - Player inventory management
- `PlayerStats.cs` - Player statistics and attributes
- `Health.cs` - Player health system
- `Joystick.cs` - Mobile joystick controls
- `VirtualJoystick.cs` - Virtual joystick implementation

### 📁 Dialogue/
**Dialogue and story systems**
- `DialogueUIController.cs` - UI controller for dialogue and story choices
- `DialogueManager.cs` - Core dialogue system management
- `DialogueData.cs` - Data structures for dialogue content
- `DialogueLoader.cs` - Basic dialogue loading from YAML
- `DialogueLoaderEnhanced.cs` - Enhanced dialogue loading features
- `DialogueTrigger.cs` - Trigger system for dialogue events
- `DialogueSystemDebugger.cs` - Debugging tools for dialogue
- `NPCDialogue.cs` - NPC-specific dialogue handling
- `DialogueExample.cs` - Example dialogue implementation
- `DialogueTextDebugger.cs` - Text debugging utilities

### 📁 Puzzles/
**Puzzle game mechanics and systems**
- `MathPuzzle3D.cs` - 3D mathematical puzzle with symbol interaction
- `MemoryPatternPuzzle.cs` - Sequence memory and pattern recognition puzzles
- `PowerFlowPuzzleSingleFile.cs` - Energy flow and connection puzzles
- `SleightGridPuzzle.cs` - Logic-based grid constraint puzzles
- `PatternElement.cs` - Individual pattern elements for memory puzzles

### 📁 Vehicle/
**Vehicle physics and AI systems**
- `SleightController.cs` - Advanced sled physics and controls
- `SleightPowerSystem.cs` - Power collection and management for sleds
- `VehicleController.cs` - General vehicle physics controller
- `CarAI.cs` - Basic car AI behavior
- `AdvancedCarAI.cs` - Advanced AI with pathfinding
- `AICarManager.cs` - AI car management and coordination
- `WaypointManager.cs` - Waypoint navigation system

### 📁 Weapons/
**Weapon systems**
- `WeaponBase.cs` - Base class for all weapons
- `Sword.cs` - Sword weapon implementation
- `Bow.cs` - Bow weapon implementation
- `Shield.cs` - Shield weapon implementation

### 📁 AI/
**AI and enemy systems**
- `EnemyAI.cs` - Basic enemy AI behavior
- `FlyingAI.cs` - Flying enemy AI behavior

### 📁 Camera/
**Camera control systems**
- `CameraFollow2D.cs` - Basic 2D camera following
- `SmoothFollowCamera2D5.cs` - Smooth 2.5D camera following
- `OffsetFollow.cs` - Camera with offset following

### 📁 UI/
**User interface systems**
- `GameHUDController.cs` - Main game HUD management
- `PuzzleHUDController.cs` - Puzzle-specific UI and progress tracking
- `PuzzleButton3D.cs` - 3D interactive button components
- `MathButton3D.cs` - 3D mathematical symbol buttons
- `MenuButton3D.cs` - 3D menu navigation buttons
- `MainMenu3D.cs` - 3D main menu interface
- `OptionsController.cs` - Options menu controller
- `OptionsMenuController.cs` - Options menu management
- `OptionsPanelController.cs` - Options panel handling
- `PauseMenu.cs` - Pause menu functionality
- `SaveLoadUI.cs` - Save/load UI interface
- `SettingsMenu.cs` - Game settings interface
- `SleightHUDExtension.cs` - Sled-specific HUD elements

### 📁 Systems/
**Game systems and utilities**
- `SettingsApplier.cs` - Apply game settings
- `SettingsMenu.cs` - Settings menu management
- `SettingsStore.cs` - Settings storage system
- `TriggerManager.cs` - General trigger system
- `TriggerPause.cs` - Pause trigger functionality
- `Checkpoint.cs` - Checkpoint system
- `Collectible.cs` - Collectible items system
- `MovingPlatform.cs` - Moving platform mechanics
- `SavePoint.cs` - Save point system
- `GameLoader.cs` - Game loading utilities
- `CutsceneManager.cs` - Cutscene and cinematic management
- `FloorFollower.cs` - Ground-following object behavior
- `GameSettings.cs` - Game configuration settings
- `ObjectSpawner.cs` - Dynamic object spawning system
- `PowerOrb.cs` - Collectible power orb implementation
- `PowerOrbSpawner.cs` - Power orb spawning and management

### 📁 Effects/
**Visual effects and post-processing**
- `FullscreenMotionBlurController.cs` - Fullscreen motion blur
- `SimpleMotionBlurController.cs` - Simple motion blur effect

## Key Features

### Puzzle System
Comprehensive puzzle mechanics featuring:
- **3D Math Puzzles**: Interactive mathematical problem solving with 3D symbols
- **Memory Pattern Puzzles**: Sequence recognition and reproduction challenges
- **Power Flow Puzzles**: Energy network manipulation and connection puzzles
- **Sleight Grid Puzzles**: Logic-based constraint satisfaction challenges
- **Progressive Difficulty**: Adaptive challenge scaling based on player progress
- **Full Transform Control**: 3D positioning, rotation, and scaling of puzzle elements

### Vehicle Physics System
Advanced sled and vehicle mechanics:
- **Realistic Sled Physics**: Authentic movement with momentum and control
- **Power Collection**: Energy orb gathering and management systems
- **Speed Progression**: From basic movement to faster-than-light travel
- **AI Racing**: Computer-controlled opponents with advanced pathfinding
- **Dynamic Spawning**: Adaptive power orb placement and generation

### Story System
Rich narrative framework with:
- **Character-Driven Dialogue**: Memorable characters like Dr. Mortimer Bones, The Nutcracker
- **Branching Narratives**: Multiple story paths based on player choices and puzzle completion
- **YAML Content**: Story content stored in YAML files for easy editing
- **Integrated Progression**: Story events unlock puzzle challenges and abilities

### Save System
- Persistent save data across sessions
- Multiple save slots support
- Auto-save functionality
- Quick save/load with F5/F9

### Player Systems
- **Advanced 3D Movement**: First-person controller with physics-based mechanics
- **Dual Input Modes**: FPV navigation and UI interaction modes (Tab to toggle)
- **Cross-Platform Support**: Keyboard, mouse, touch, and virtual joystick controls
- **Inventory System**: Weapon switching and item management
- **Experience System**: Player progression and leveling mechanics
- **Health and Stats**: Comprehensive player attribute tracking

### Input Management
Advanced input handling system:
- **Mode Switching**: Seamless transition between FPV and UI interaction
- **Cursor Control**: Intelligent cursor lock management for 3D interactions
- **Mobile Support**: Virtual joysticks and touch-friendly controls
- **3D Interaction**: Direct manipulation of 3D puzzle elements and symbols

### Dialogue System
- Rich dialogue with character portraits and animations
- Choice-based interactions
- Integration with story progression
- Typing effects and UI animations

## Usage Instructions

### Basic Setup
1. Add `DialogueManager` and `DialogueUIController` to scene
2. Create UIDocument with `DialogueSystem.uxml`
3. Link UIDocument to `DialogueUIController`
4. Create YAML dialogue files
5. Assign dialogue files to NPCs via `NPCDialogue` component

### Key Features
- **Linear Dialogue**: ✅ Fully supported
- **YAML Data**: ✅ Easy to edit dialogue files
- **Visual Novel UI**: ✅ Professional dialogue interface
- **3D Integration**: ✅ Speech bubbles and world positioning
- **Choice Support**: ⚠️ Requires C# implementation for branching logic

### Important Notes
- **Choices are display-only** - implement logic in C# for actual branching
- **Use linear dialogue** for simple conversations
- **YAML files** can be placed in StreamingAssets or Resources folders
- **Debug tools** available for testing and troubleshooting

## Example YAML Structure
```yaml
scene: "example_dialogue"
lines:
  - id: "greeting"
    speaker: "NPC Name"
    text: "Hello, traveler!"
    animation: "happy"
  
  - id: "question"
    speaker: "NPC Name"
    text: "How can I help you?"
    animation: "curious"
```

## Related Files
- `UI/DialogueSystem.uxml` - UI layout
- `UI/DialogueSystem.uss` - UI styling
- `example_dialogue.yaml` - Simple example
- `village_elder_dialogue.yaml` - Complex example
- `DIALOGUE_CHOICES_README.md` - Detailed choice implementation guide

## Integration with Game Systems
The dialogue system integrates with:
- **Quest System** - via `quest_id` field in dialogue lines
- **Trigger System** - via `trigger` field for game events
- **Animation System** - via `animation` field for character expressions
- **Story System** - via `StoryRouteManager` for story progression

For detailed implementation instructions, see `DIALOGUE_CHOICES_README.md` in the Assets folder.
