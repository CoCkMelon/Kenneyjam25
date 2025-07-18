# Save System Setup Guide

## Overview
This document provides a comprehensive guide for implementing the save system in your Unity 2.5D platformer project.

## Scripts Overview

### Core Save System Scripts
1. **SaveData.cs** - Contains all serializable game data
2. **SaveSystem.cs** - Handles file I/O operations
3. **GameLoader.cs** - Manages game state loading and restoration
4. **SavePoint.cs** - Interactive save points for players
5. **Checkpoint.cs** - Automatic save triggers
6. **Health.cs** - Player health system with save integration
7. **SaveLoadUI.cs** - UI interface for save/load operations

### Updated Scripts
- **GameManager.cs** - Added auto-save and quick save/load
- **PlayerInventory.cs** - Added inventory save/load support
- **QuestManager.cs** - Quest state persistence (removed)

## Setup Instructions

### 1. Player Setup
- Ensure your player GameObject has the "Player" tag
- Add the following components to your player:
  - `PlayerController` (existing)
  - `Health` (new)
  - `PlayerInventory` (updated)

### 2. GameManager Setup
- Add the updated `GameManager` script to a GameObject in your scene
- Configure auto-save settings in the inspector
- Set auto-save interval (default: 300 seconds = 5 minutes)

### 3. Save Point Setup
Create a save point prefab:
```
SavePoint GameObject
├── SavePoint script
├── Collider (Is Trigger = true)
├── Visual representation (optional)
├── Particle System (optional)
├── Audio Source (optional)
└── UI Canvas (optional)
    └── Interact Prompt Text
```

**SavePoint Inspector Settings:**
- Save Point ID: Unique identifier (e.g., "forest_savepoint_01")
- Is Activated: Usually false initially
- Auto Save: True for automatic saving on approach

### 4. Checkpoint Setup
Create a checkpoint prefab:
```
Checkpoint GameObject
├── Checkpoint script
├── Collider (Is Trigger = true)
└── Visual representation (optional)
```

**Checkpoint Inspector Settings:**
- Checkpoint ID: Unique identifier (e.g., "forest_checkpoint_01")

### 5. Scene Setup
- Add `GameLoader` script to a persistent GameObject
- Configure default scene and spawn point
- Place save points and checkpoints throughout your levels

### 6. UI Setup
Create a save/load UI with the following structure:
```
SaveLoadUI Canvas
├── Save Button
├── Load Button
├── New Game Button
├── Delete Save Button
├── Save Status Text
└── Save Info Panel
    ├── Scene Name Text
    ├── Playtime Text
    ├── Last Save Time Text
    └── Level Info Text
```

## Implementation Steps

### Step 1: Basic Integration
1. Add all scripts to your project
2. Set up the player with required components
3. Create and place a simple save point in your scene
4. Test basic save/load functionality

### Step 2: Advanced Features
1. Set up auto-save in GameManager
2. Create checkpoint system for respawning
3. Implement UI for save/load operations
4. Add visual/audio feedback for save points

### Step 3: Testing
1. Test save/load across different scenes
2. Verify inventory and quest persistence
3. Test respawn functionality
4. Validate auto-save behavior

## Key Bindings
- **F5**: Quick Save
- **F9**: Quick Load
- **E**: Interact with save points (when in range)

## Save File Location
- **Windows**: `%USERPROFILE%/AppData/LocalLow/[CompanyName]/[ProductName]/Saves/`
- **Mac**: `~/Library/Application Support/[CompanyName]/[ProductName]/Saves/`
- **Linux**: `~/.config/unity3d/[CompanyName]/[ProductName]/Saves/`

## Troubleshooting

### Common Issues
1. **Save file not found**: Ensure save directory exists and permissions are correct
2. **Player not respawning**: Check if player has "Player" tag and Health component
3. **Save points not triggering**: Verify colliders are set to "Is Trigger"
4. **Data not persisting**: Check if all required components are properly configured

### Debug Tips
- Enable debug logs in the console to track save/load operations
- Use the Unity Inspector to monitor save data fields
- Test with different scenes and player states

## Best Practices

### Performance
- Limit auto-save frequency to avoid performance issues
- Use object pooling for frequently created save-related objects
- Consider async save operations for large save files

### User Experience
- Provide clear visual feedback when saving
- Show save progress for long operations
- Allow players to save manually at any time
- Implement save file corruption recovery

### Development
- Use unique IDs for all save points and checkpoints
- Test save/load functionality regularly during development
- Document save point locations and purposes
- Version your save data format for future updates

## Future Enhancements
- Multiple save slots
- Cloud save integration
- Save file encryption
- Compressed save files
- Save file versioning
- Auto-backup system
