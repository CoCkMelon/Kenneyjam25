# Core Systems

This folder contains the core game systems and managers that form the foundation of the game.

## Scripts

### GameManager.cs
Main game state controller that manages:
- Game score and lives
- Game time tracking
- Pause/unpause functionality
- Checkpoint system
- Auto-save functionality
- Game over and restart logic

**Key Features:**
- Singleton pattern for global access
- Auto-save every 5 minutes
- Quick save/load with F5/F9
- Checkpoint respawn system

### SaveSystem.cs & SaveData.cs
Persistent save/load system:
- JSON-based save files
- Multiple save slots
- Automatic directory creation
- Data validation

**Usage:**
```csharp
SaveData data = SaveSystem.LoadGame();
data.playerPosition = player.transform.position;
SaveSystem.SaveGame(data);
```

### QuestManager.cs (Removed)
~~Quest tracking system - removed for reimplementation~~

### ExperienceSystem.cs
Player progression system:
- Level-based progression
- Experience points calculation
- Stat bonuses per level
- Level-up effects and events

**Features:**
- 50 level cap
- Exponential XP scaling
- Stat bonuses for health, mana, attack, defense
- Experience from combat, quests, and discoveries

### ActivityScheduler.cs (Removed)
~~Activity scheduling system - removed for reimplementation~~

### StoryRouteManager.cs
Story progression and choice management:
- Hardcoded story routes with if/else logic
- Choice-based story branching
- Multiple ending determination
- Value-based story outcomes

**Key Features:**
- 10 story locations with branching paths
- 5 different endings based on player choices
- Harmony-based "perfect" ending
- Integration with moral values system

### YAMLContentLoader.cs
YAML content loading utility:
- Loads story content from YAML files
- Supports story locations and descriptions
- Simple deserialization without external dependencies

## Dependencies
- Unity 2022.3 LTS or later
- TextMeshPro for UI text
- Optional: YamlDotNet for YAML parsing

## Setup Instructions
1. Add GameManager to your main scene
2. Configure checkpoint transforms
3. Set up UI references for score, lives, time display
4. Configure save system paths
5. Initialize story system with StoryRouteManager
