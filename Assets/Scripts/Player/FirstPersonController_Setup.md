# First-Person Controller Setup Guide

This guide explains how to set up the first-person controller system with mobile HUD controls integration.

## Overview

The system consists of several interconnected components:
- **FirstPersonController**: Main movement and camera control
- **PlayerInput**: Unified input handling for desktop and mobile
- **PlayerController**: Wrapper for HUD system integration
- **VirtualJoystick**: Touch-based joystick for mobile devices
- **Health, PlayerStats, PlayerInventory**: Supporting systems

## Setup Instructions

### 1. Player GameObject Setup

1. Create a new GameObject named "Player"
2. Add the following components in order:
   - **CharacterController** (Unity built-in)
   - **PlayerController** (our wrapper script)
   - **FirstPersonController** (main movement script)
   - **PlayerInput** (input handling)
   - **Health** (health system)
   - **PlayerStats** (statistics)
   - **PlayerInventory** (inventory system)

### 2. Character Controller Configuration

Configure the CharacterController component:
- **Height**: 2.0
- **Radius**: 0.5
- **Center**: (0, 1, 0)
- **Slope Limit**: 45
- **Step Offset**: 0.3

### 3. Camera Setup

1. Create a child GameObject named "PlayerCamera"
2. Add a **Camera** component
3. Position it at (0, 1.6, 0) relative to the Player
4. Set the camera's tag to "MainCamera"

### 4. Ground Check Setup

1. Create a child GameObject named "GroundCheck"
2. Position it at (0, -1, 0) relative to the Player
3. Assign this to the **Ground Check** field in FirstPersonController

### 5. FirstPersonController Configuration

Configure the FirstPersonController component:
- **Walk Speed**: 5.0
- **Run Speed**: 10.0
- **Jump Force**: 5.0
- **Gravity**: 9.81
- **Air Control**: 0.3
- **Mouse Sensitivity**: 2.0
- **Touch Sensitivity**: 2.0
- **Max Look Angle**: 80
- **Ground Distance**: 0.4
- **Ground Mask**: Set to "Ground" layer

### 6. Mobile Controls Integration

The system automatically integrates with the existing GameHUDController. The mobile controls are already defined in the GameHUD.uxml file:

- **D-Pad buttons**: For movement (dpad-up, dpad-left, dpad-right, dpad-down)
- **Action buttons**: For jump, dash, attack, weapon switching (action-button-a, action-button-b, action-button-x, action-button-y)

### 7. Virtual Joystick Setup (Optional)

If you want to use virtual joysticks instead of D-Pad buttons:

1. Create UI elements for joystick backgrounds and handles
2. Add **VirtualJoystick** components to the background elements
3. Assign the handles to the **Joystick Handle** fields
4. Link the joysticks to the **PlayerInput** component

### 8. Layer Setup

Create and configure these layers:
- **Ground**: For walkable surfaces
- **Player**: For the player GameObject

### 9. Input Settings

The system works with Unity's default Input Manager settings:
- **Horizontal**: A/D keys and Left/Right arrows
- **Vertical**: W/S keys and Up/Down arrows
- **Jump**: Space key
- **Mouse X**: Mouse horizontal movement
- **Mouse Y**: Mouse vertical movement

## Features

### Desktop Controls
- **WASD / Arrow Keys**: Movement
- **Mouse**: Look around
- **Space**: Jump
- **Shift**: Run
- **1, 2, 3**: Weapon switching
- **P**: Pause
- **I**: Inventory
- **Q**: Quest log

### Mobile Controls
- **D-Pad**: Movement
- **Action Button A**: Jump
- **Action Button B**: Dash
- **Action Button X**: Attack
- **Action Button Y**: Weapon switching
- **Touch joysticks**: Alternative movement and look controls

### Automatic Platform Detection
The system automatically detects mobile platforms and adjusts:
- Cursor locking (disabled on mobile)
- Input method selection
- UI element visibility

## Integration with Existing Systems

### HUD Integration
The PlayerInput system integrates seamlessly with the existing GameHUDController:
- Button presses are handled through the HUD system
- Mobile controls visibility is managed automatically
- Settings integration for virtual joystick preferences

### Save System Integration
The player systems integrate with the existing save system:
- Player position and stats are saved
- Inventory and weapon states persist
- Health and experience are maintained

### Quest System Integration
The player can interact with the quest system:
- Quest completion triggers
- Experience rewards
- Item collection

## Troubleshooting

### Common Issues

1. **Player falls through floor**
   - Check Ground Mask settings
   - Ensure Ground Check position is correct
   - Verify layer assignments

2. **Mobile controls not working**
   - Check that GameHUDController is properly set up
   - Ensure PlayerInput component is assigned
   - Verify button names match the UXML file

3. **Camera rotation issues**
   - Check camera parent hierarchy
   - Verify mouse sensitivity settings
   - Ensure camera is child of Player GameObject

4. **Input not responding**
   - Check PlayerInput component assignment
   - Verify Input Manager settings
   - Ensure no conflicting input systems

### Debug Tips

1. Enable debug logs in the scripts to trace input flow
2. Use the Unity Profiler to check performance
3. Test on actual mobile devices, not just the editor
4. Check the GameHUDController for proper button assignments

## Customization

### Adding New Actions
1. Add input handling to PlayerInput
2. Add corresponding methods to FirstPersonController
3. Update GameHUDController button assignments
4. Add UI elements to GameHUD.uxml if needed

### Modifying Movement
- Adjust speed values in FirstPersonController
- Modify gravity and jump force for different feel
- Change air control for more/less aerial maneuverability

### Customizing Mobile Controls
- Modify GameHUD.uxml for different button layouts
- Adjust VirtualJoystick settings for sensitivity
- Change button icons and styling in GameHUD.uss

## Performance Considerations

- The system is optimized for mobile performance
- Virtual joysticks only update when being used
- Input polling is minimized
- UI updates are batched where possible

## Future Enhancements

Potential improvements:
- Gamepad support integration
- Advanced movement mechanics (wall-running, sliding)
- Customizable input mapping
- Gesture-based controls for mobile
- Haptic feedback integration
