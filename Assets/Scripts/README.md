# OFL Unity Project - Script Organization

This document describes the organization and structure of the Unity project scripts.

## Project Structure

The scripts are organized into the following directories:

### 📁 Core/
**Core game systems and managers**
- `GameManager.cs` - Main game state management, score, lives, checkpoints
- `SaveSystem.cs` - Save/load game data functionality
- `SaveData.cs` - Data structures for save system
- `QuestManager.cs` - Quest system management (removed)
- `ExperienceSystem.cs` - Player experience and leveling system
- `ActivityScheduler.cs` - Activity scheduling system with moral values (removed)
- `StoryRouteManager.cs` - Story progression and choice management
- `YAMLContentLoader.cs` - YAML content loading for stories

### 📁 Player/
**Player-related scripts**
- `PlayerController.cs` - Main player movement and controls
- `PlayerInput.cs` - Input handling for player actions
- `PlayerInventory.cs` - Player inventory management
- `PlayerStats.cs` - Player statistics and attributes
- `Health.cs` - Player health system

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
- `OptionsController.cs` - Options menu controller
- `OptionsMenuController.cs` - Options menu management
- `OptionsPanelController.cs` - Options panel handling
- `PauseMenu.cs` - Pause menu functionality
- `SaveLoadUI.cs` - Save/load UI interface

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

### 📁 Effects/
**Visual effects and post-processing**
- `FullscreenMotionBlurController.cs` - Fullscreen motion blur
- `SimpleMotionBlurController.cs` - Simple motion blur effect

## Key Features

### Story System
The project includes a comprehensive story system with:
- **Moral Values Tracking**: 15 different values including honesty, courage, logic, etc.
- **Branching Narratives**: Multiple story paths based on player choices
- **YAML Content**: Story content stored in YAML files for easy editing
- **Harmony System**: Balance between different moral aspects

### Save System
- Persistent save data across sessions
- Multiple save slots support
- Auto-save functionality
- Quick save/load with F5/F9

### Player Systems
- Advanced 2.5D movement with wall-jumping, dashing, and climbing
- Inventory system with weapon switching
- Experience and leveling system
- Health and statistics tracking

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
