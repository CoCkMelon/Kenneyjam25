# Vehicle System Documentation

## Overview
This vehicle system provides a primitive car controller with AI navigation capabilities. The system includes:

- **VehicleController**: Basic car physics and movement
- **CarAI**: Simple AI using NavMesh
- **AdvancedCarAI**: More sophisticated AI with pathfinding and obstacle avoidance
- **WaypointManager**: Manages waypoints for AI navigation
- **AICarManager**: Combines AI and waypoint systems

## Setup Instructions

### 1. Basic Vehicle Setup
1. Create a car GameObject with a Rigidbody component
2. Add 4 WheelCollider components for the wheels
3. Add the `VehicleController` script
4. Assign the wheel colliders and wheel transforms in the inspector
5. Set the center of mass transform for better physics

### 2. AI Setup
Choose one of the AI options:

#### Option A: Simple AI (CarAI)
1. Add the `CarAI` script to your vehicle
2. Set the destination point in the inspector
3. The AI will automatically navigate using NavMesh

#### Option B: Advanced AI (AdvancedCarAI)
1. Add the `AdvancedCarAI` script to your vehicle
2. Set the destination point in the inspector
3. Configure AI settings (speed, detection distance, etc.)
4. The AI will create waypoints and navigate with obstacle avoidance

### 3. Waypoint System
1. Create empty GameObjects for waypoints
2. Tag them with "Waypoint" or manually assign them
3. Add `WaypointManager` script to your vehicle
4. Add `AICarManager` script for automated waypoint navigation

## Usage

### Manual Control
- **Arrow Keys / WASD**: Steer and accelerate
- **Space**: Brake
- **T**: Toggle AI on/off (when AICarManager is present)
- **N**: Go to next waypoint (when using waypoints)

### Code Examples

#### Set AI Destination
```csharp
// Set destination by transform
carAI.SetDestination(targetTransform);

// Set destination by position
carAI.SetDestination(new Vector3(10, 0, 20));
```

#### Check AI Status
```csharp
bool hasReached = carAI.HasReachedDestination();
float distance = carAI.GetDistanceToDestination();
```

#### Control AI Manager
```csharp
AICarManager manager = GetComponent<AICarManager>();
manager.SetDestination(newTarget);
manager.ToggleAI();
manager.StopNavigation();
```

## Configuration

### VehicleController Settings
- **Motor Force**: Power of the engine
- **Brake Force**: Braking strength
- **Max Steer Angle**: Maximum steering angle
- **Max Speed**: Speed limit
- **Down Force**: Downward force for grip

### AI Settings
- **Stopping Distance**: How close to get to destination
- **Max Speed**: AI speed limit
- **Obstacle Detection Distance**: Range for obstacle detection
- **Steer Sensitivity**: How responsive the steering is

## Tips

1. **Physics**: Adjust the center of mass for better vehicle stability
2. **Wheels**: Make sure wheel colliders are properly positioned
3. **Layers**: Use layers to separate obstacles from navigation areas
4. **NavMesh**: For CarAI, bake a NavMesh in your scene
5. **Waypoints**: Name waypoints sequentially for proper ordering

## Troubleshooting

- **Car flips over**: Lower the center of mass
- **AI doesn't move**: Check if destination is set and reachable
- **Wheels don't turn**: Verify wheel transforms are assigned
- **NavMesh issues**: Ensure NavMesh is baked for the scene
- **AI gets stuck**: Increase obstacle detection distance

## Advanced Features

The AdvancedCarAI includes:
- Dynamic pathfinding without NavMesh
- Obstacle avoidance with raycasting
- Speed control based on distance to destination
- Debug visualization for paths and detection rays
- Waypoint-based navigation with the WaypointManager

## Performance Notes

- The AdvancedCarAI does more calculations per frame
- Consider using the simple CarAI for multiple vehicles
- Adjust update frequencies if performance is an issue
- Use object pooling for multiple AI vehicles
