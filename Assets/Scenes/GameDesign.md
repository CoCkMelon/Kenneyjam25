# Game Object Hierarchy and Components for Sleight Game

## Main Game Objects

### Sleight (Main Player)
- **Components:**
  - `SleightController` (Main controller)
  - `SleightPowerSystem` (Power collection system)
  - `PlayerInput` (Input handling)
  - `Rigidbody` (Physics)
  - `AudioSource` (Audio effects)
  - `Collider` (Collision detection)
- **Child Objects:**
  - **MovementTrail** (ParticleSystem)
  - **PowerCollectionEffect** (ParticleSystem)
  - **VisualModel** (3D model/mesh)

### Game Systems
- **CutsceneManager**
  - `CutsceneManager` component
  - **Child Objects:**
    - **LightSpeedCutsceneCamera** (Camera)
    - **LightSpeedEffect** (ParticleSystem)
    - **ScreenEffects** (Post-processing)
    - **CutsceneUI** (UI overlay)

- **PowerOrbSpawner**
  - `PowerOrbSpawner` component
  - **Child Objects:**
    - **PowerOrb Prefabs** (instantiated at runtime)

### UI System
- **GameHUD**
  - `GameHUDController` (existing)
  - `SleightHUDExtension` (new sleight-specific UI)
- **PauseMenu** (existing)
- **CutsceneUI** (for cutscenes)

## Component Details

### SleightController
- **Movement Settings:**
  - Base Speed: 5
  - Max Speed: 1000 (light speed threshold)
  - Acceleration: 2
  - Turn Speed: 180
  - Auto Move: True
  - Auto Move Input Delay: 0.5s
- **Power System:**
  - Power Level: 0
  - Max Power Level: 1000
  - Power to Speed Multiplier: 10
  - Power to Strength Multiplier: 5
- **Auto-Movement:**
  - Obstacle Detection Distance: 3
  - Obstacle Layer: -1
- **Events:**
  - OnPowerChanged
  - OnSpeedChanged
  - OnLightSpeedReached

### SleightPowerSystem
- **Power Settings:**
  - Current Power: 0
  - Max Power: 1000
  - Power Decay Rate: 0.5
  - Enable Power Decay: False
- **Power Collection:**
  - Collection Radius: 2
  - Power Orb Layer: -1
  - Magnetic Range: 5
  - Magnetic Strength: 10
- **Power Multipliers:**
  - Base Multiplier: 1
  - Max Combo Multiplier: 5
  - Combo Decay Time: 2s
- **Events:**
  - OnPowerCollected
  - OnPowerLevelChanged
  - OnComboChanged
  - OnMaxPowerReached

### PowerOrb
- **Orb Settings:**
  - Power Value: 10-250 (depends on type)
  - Orb Types: Basic, Enhanced, Rare, Epic, Legendary
  - Life Time: 30s
- **Movement:**
  - Float Amplitude: 0.5
  - Float Speed: 2
  - Rotation Speed: 90
- **Magnetic Properties:**
  - Magnetic Resistance: 1
  - Max Magnetic Speed: 10
- **Effects:**
  - Collect Effect: ParticleSystem
  - Audio Source: AudioSource
  - Visual Model: GameObject

### PowerOrbSpawner
- **Spawning Settings:**
  - Spawn Rate: 2 orbs/second
  - Max Orbs: 20
  - Spawn Radius: 50
  - Min Spawn Distance: 10
- **Spawn Area:**
  - Size: (100, 20, 100)
  - Ground Layer: -1
  - Ground Check Distance: 10
- **Dynamic Spawning:**
  - Follow Player: True
  - Update Interval: 1s
  - Adaptive Spawning: True
  - Adaptive Spawn Rate Multiplier: 1.5

### CutsceneManager
- **Light Speed Cutscene:**
  - Cutscene Duration: 10s
  - Camera Start Offset: (0, 5, -10)
  - Camera End Offset: (0, 20, -5)
  - Camera Rotation Speed: 45°/s
- **Post-Processing:**
  - Effect Intensity: 1
  - Effect Curve: EaseInOut
- **Events:**
  - OnCutsceneStart
  - OnCutsceneEnd
  - OnLightSpeedSequenceComplete

### SleightHUDExtension
- **UI Elements:**
  - Power Bar Fill
  - Power Text
  - Speed Bar Fill
  - Speed Text
  - Combo Container
  - Combo Text
  - Status Text
  - Light Speed Indicator
- **Features:**
  - Real-time power/speed display
  - Combo multiplier visualization
  - Color-coded progress bars
  - Light speed warning indicators

## Input & HUD Integration
- Uses existing `PlayerInput` system
- Integrates with existing `GameHUDController`
- Adds sleight-specific UI elements via `SleightHUDExtension`
- Supports both manual and auto-movement modes
- Compatible with existing mobile controls

## Cutscene System
- Triggered when speed >= 99% of max speed (990/1000)
- Disables main camera, enables cutscene camera
- Plays particle effects and music
- Slows time to 0.1x for dramatic effect
- Rotates camera around player
- Automatically ends after duration
- Supports skip functionality

## Power Collection System
- Magnetic attraction draws orbs to player
- Different orb types provide different power amounts
- Combo system multiplies collection effectiveness
- Adaptive spawning increases orb spawn rate as player gets stronger
- Ground-based spawning with obstacle avoidance
