# 🚁 Kenney Jam 25 - Surreal Sled Adventure

> **A Unity 3D Adventure Game featuring Surreal Story-driven Gameplay, Puzzle Mechanics, and High-Speed Sled Physics**

![Unity Version](https://img.shields.io/badge/Unity-2023.3%2B-blue.svg)
![License](https://img.shields.io/badge/License-MIT-green.svg)
![Platform](https://img.shields.io/badge/Platform-Windows%20%7C%20Linux%20%7C%20macOS-lightgrey.svg)

## 🌟 Overview

**Kenneyjam25** is a surreal adventure game built for Unity, combining high-speed sled physics with mind-bending puzzle mechanics and an engaging narrative featuring eccentric characters like Dr. Mortimer Bones, The Nutcracker, and Cornelius Frost. Players navigate through a quantum reality where sleds can achieve faster-than-light travel through mastering various puzzle challenges.

## 🎮 Key Features

### 🏎️ **Advanced Vehicle Physics**
- **Sled Controller**: Realistic sled physics with power collection systems
- **AI Car Management**: Advanced AI for racing and chase sequences  
- **Speed Systems**: Dynamic speed mechanics culminating in light-speed travel

### 🧩 **Comprehensive Puzzle System**
- **Memory Pattern Puzzles**: Test recall and sequence recognition
- **Power Flow Puzzles**: Manipulate energy networks and connections
- **Sleight Grid Puzzles**: Logic-based constraint satisfaction challenges
- **3D Math Puzzles**: Interactive mathematical problem solving with 3D symbols
- **Positioning Control**: Full 3D transform control for puzzle elements

### 🎭 **Rich Narrative System**
- **YAML-Based Dialogue**: Easy-to-edit story content and character interactions
- **Branching Narratives**: Multiple story paths with character choices
- **Character System**: Memorable cast including skeleton mages and magical nutcrackers
- **Cutscene Management**: Dynamic story sequences and transitions

### 🎯 **Input & Interaction**
- **Dual Input Modes**: FPV navigation and UI interaction modes (Tab to toggle)
- **Mouse Control Fix**: Resolved cursor lock conflicts for seamless 3D interactions
- **Cross-Platform Input**: Support for keyboard, mouse, and touch controls
- **3D Symbol Interaction**: Direct manipulation of 3D mathematical symbols

### 💾 **Robust Save System**
- **Multiple Save Slots**: Comprehensive game state persistence
- **Auto-Save Functionality**: Seamless progress preservation
- **Quick Save/Load**: Instant save states with F5/F9
- **Progress Tracking**: Detailed statistics and achievement tracking

## 🏗️ Project Structure

```
Kenneyjam25/
├── 📁 Assets/
│   ├── 📁 Animations/          # Character and UI animations
│   ├── 📁 Audio/               # Sound effects and music
│   ├── 📁 Materials/           # 3D materials and shaders
│   ├── 📁 Models/              # 3D models and meshes
│   ├── 📁 Prefabs/             # Reusable game objects
│   ├── 📁 Scenes/              # Game scenes and levels
│   │   ├── CarScene1.unity     # Vehicle racing scene
│   │   ├── DialogueTest.unity  # Dialogue system testing
│   │   ├── MainMenu.unity      # Main menu interface
│   │   ├── PuzzleScene.unity   # Puzzle game scene
│   │   └── SleighSpeed.unity   # High-speed sled scene
│   ├── 📁 Scripts/             # C# game logic (69+ scripts)
│   │   ├── 📁 Core/           # Game managers and systems
│   │   ├── 📁 Player/         # Player controls and stats
│   │   ├── 📁 Puzzles/        # Puzzle game mechanics
│   │   ├── 📁 Vehicle/        # Sled and car controllers
│   │   ├── 📁 Dialogue/       # Story and conversation systems
│   │   ├── 📁 UI/             # User interface controllers
│   │   └── 📁 Systems/        # Utility and helper systems
│   ├── 📁 Settings/            # Project configuration
│   ├── 📁 StoryRoutes/         # YAML story content
│   └── 📁 UI/                  # UI layouts and styling
└── 📁 ProjectSettings/         # Unity project configuration
```

## 🎲 Game Mechanics

### Puzzle Systems
1. **Memory Pattern Puzzles** - Watch sequences, reproduce patterns
2. **Power Flow Puzzles** - Connect energy nodes and manage power distribution  
3. **Sleight Grid Puzzles** - Logic-based grid constraint solving
4. **3D Math Puzzles** - Interactive arithmetic with 3D number/symbol manipulation

### Vehicle Systems
- **Sled Physics**: Realistic movement with power collection
- **Speed Progression**: From basic movement to light-speed travel
- **AI Racing**: Computer-controlled opponents and chase sequences

### Story Integration
- **Character Progression**: Unlock abilities through story advancement
- **Moral Choices**: Decisions affect story branches and outcomes
- **Puzzle Integration**: Story events trigger puzzle challenges

## 🚀 Getting Started

### Prerequisites
- **Unity 2023.3.x** or newer
- **Universal Render Pipeline (URP)** 17.0.4+
- **.NET Framework 4.x** or higher

### Installation
1. Clone the repository:
   ```bash
   git clone [repository-url]
   cd Kenneyjam25
   ```

2. Open in Unity Hub:
   - Add project from disk
   - Select the `Kenneyjam25` folder
   - Ensure Unity version compatibility

3. Install Dependencies:
   - Universal Render Pipeline
   - 2D Sprite packages
   - Audio packages (if needed)

### Quick Start
1. Open `MainMenu.unity` scene
2. Press Play to test the main menu
3. Navigate to `PuzzleScene.unity` for puzzle gameplay
4. Try `SleighSpeed.unity` for vehicle physics

## 🎨 Key Components

### Core Systems
- **`PuzzleGameManager`** - Manages puzzle flow and progression
- **`GameManager`** - Central game state management  
- **`SaveSystem`** - Persistent game data handling
- **`StoryRouteManager`** - Narrative progression control

### Player Systems  
- **`FirstPersonController`** - 3D player movement
- **`InputModeController`** - FPV/UI mode switching
- **`PlayerInput`** - Centralized input handling

### Vehicle Systems
- **`SleightController`** - Advanced sled physics
- **`VehicleController`** - General vehicle mechanics
- **`SleightPowerSystem`** - Power collection and management

### Puzzle Systems
- **`MathPuzzle3D`** - 3D mathematical puzzle controller
- **`MemoryPatternPuzzle`** - Sequence memory challenges
- **`PowerFlowPuzzleSingleFile`** - Energy flow puzzles
- **`SleightGridPuzzle`** - Logic grid puzzles

## 🎮 Controls

### Navigation
- **WASD** / **Arrow Keys** - Movement
- **Mouse** - Look around (FPV mode)
- **Tab** - Toggle between FPV and UI interaction modes
- **Space** - Jump/Action
- **Shift** - Run/Sprint

### Puzzle Interaction
- **Mouse Click** - Interact with 3D elements
- **Click & Drag** - Manipulate puzzle components  
- **H** - Request puzzle hint
- **R** - Reset current puzzle
- **Esc** - Pause/Menu

### System Controls
- **F5** - Quick Save
- **F9** - Quick Load
- **Esc** - Pause Menu

## 🔧 Development Notes

### Recent Additions
- **3D Math Puzzle System** with symbol-based interaction
- **Input Mode Controller** for seamless FPV/UI switching  
- **Enhanced Positioning Control** for puzzle elements
- **Cursor Lock Management** fixes for 3D interactions

### Code Architecture
- **Modular Design**: Systems are loosely coupled and reusable
- **Event-Driven**: Uses UnityEvents for system communication
- **YAML Integration**: Story content externalized for easy editing
- **Save System**: Comprehensive state persistence across sessions

### Performance Considerations
- **Object Pooling**: Reuses game objects where possible
- **LOD Systems**: Distance-based detail optimization
- **Efficient Raycasting**: Optimized 3D interaction detection

## 📚 Documentation

- **[Scripts Overview](Assets/Scripts/README.md)** - Detailed script organization
- **[Game Design](Assets/Scenes/GameDesign.md)** - Core gameplay mechanics  
- **[Puzzle Design](Assets/Scenes/PuzzleDesign.md)** - Puzzle system architecture
- **[Dialogue System Setup](Assets/Scripts/README_DialogueHUD_Setup.md)** - Story system configuration
- **[Save System Setup](Assets/Scripts/SaveSystemSetup.md)** - Save/load implementation

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit changes (`git commit -m 'Add AmazingFeature'`)
4. Push to branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 🎯 Roadmap

### Completed Features ✅
- Core puzzle systems (Memory, Power Flow, Sleight Grid, Math)
- Vehicle physics and AI systems
- Save/load functionality  
- Dialogue and story systems
- Input mode management
- 3D symbol interaction

### In Development 🚧
- Enhanced visual effects
- Additional puzzle variations
- Expanded story content
- Mobile platform optimization

### Planned Features 📋
- Multiplayer puzzle challenges
- Level editor functionality
- Steam integration
- Additional vehicle types

## 🛠️ Technical Stack

- **Engine**: Unity 2023.3.x
- **Rendering**: Universal Render Pipeline (URP)
- **Physics**: Unity Physics 3D
- **Audio**: Unity Audio System
- **Data Format**: YAML for story content
- **Save System**: Binary serialization
- **UI Framework**: UI Toolkit + uGUI hybrid

## 📊 Project Statistics

- **C# Scripts**: 69+ files
- **Unity Scenes**: 5 main scenes
- **Story Content**: 10+ YAML story files
- **Puzzle Types**: 4 distinct systems
- **Character System**: Multiple NPCs with dialogue
- **Save Slots**: Multiple persistent save support

## 🎪 Game Characters

- **Dr. Mortimer Bones** - Skeleton mage and scientific advisor
- **The Nutcracker** - Mystical guide with theatrical flair  
- **Cornelius Frost** - Ancient snowman with eternal wisdom
- **Sled Operator** - Player character navigating quantum realities

## 🌟 Unique Features

- **Quantum Sled Physics** - Realistic to faster-than-light travel
- **3D Math Interactions** - Touch and manipulate mathematical symbols
- **Dual Input Modes** - Seamless switching between navigation and interaction
- **Story-Puzzle Integration** - Narrative directly affects puzzle availability
- **Character-Driven Hints** - In-universe characters provide contextual help

## 📞 Support

For questions, bug reports, or feature requests:
- Create an issue in the repository
- Check existing documentation in the `Assets/` folder
- Review the comprehensive README files in each subdirectory

---

**Built with ❤️ for Kenney Jam 25**

*A surreal adventure where sleds defy physics and puzzles reshape reality!*
