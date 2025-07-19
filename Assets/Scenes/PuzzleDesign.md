# Sleight-Themed Puzzle Systems

## Overview
These puzzle systems are designed around the theme of **sleight of hand** and **power manipulation**. Each puzzle requires mouse/touch interaction and focuses on skill, timing, and strategic thinking rather than physics-based mechanics.

## Puzzle 1: Power Circuit Puzzle
**Theme**: "Channel power through sleight of hand to activate mechanisms"

### Concept
Connect power nodes by clicking and dragging to create circuits. Power flows from source nodes to end nodes, and the player must create the correct connections to complete the puzzle.

### Mechanics
- **Click and drag** from one power node to another to create connections
- **Visual feedback** shows connection lines and power flow
- **Power flows** from Start nodes through Regular nodes to End nodes
- **Limited range** - nodes must be within connection range
- **Multiple solutions** or specific path requirements

### Components
- **PowerCircuitPuzzle** - Main puzzle controller
- **PowerNode** - Individual connectable nodes
- **PowerConnection** - Data structure for connections

### Node Types
- **Start**: Power source (green)
- **End**: Power destination (red)
- **Regular**: Pass-through node (blue)
- **Amplifier**: Boosts power signal
- **Splitter**: Allows multiple connections

### Visual Effects
- Glowing connections when powered
- Pulsing nodes when active
- Particle effects for power flow
- Color-coded materials for different states

---

## Puzzle 2: Memory Pattern Puzzle
**Theme**: "Master the sleight of hand by remembering and repeating power patterns"

### Concept
Watch a sequence of power activations, then recreate it by clicking the elements in the correct order. Difficulty increases with longer patterns.

### Mechanics
- **Watch phase**: Elements light up in sequence
- **Input phase**: Click elements to repeat the pattern
- **Progressive difficulty**: Patterns get longer each round
- **Time pressure**: Limited time to input the sequence
- **Replay option**: Limited replays available

### Components
- **MemoryPatternPuzzle** - Main puzzle controller
- **PatternElement** - Individual clickable elements

### Element Types
- **Standard**: Basic white element
- **Fire**: Red element with fire particles
- **Water**: Blue element with water effects
- **Earth**: Green element with earth textures
- **Air**: Cyan element with wind effects
- **Light**: Yellow element with bright glow
- **Dark**: Magenta element with shadow effects

### Features
- **Audio cues**: Different sound for each element
- **Visual feedback**: Pulsing and scaling animations
- **Error indication**: Red flash when wrong
- **Completion celebration**: All elements glow green

---

## Puzzle 3: Power Flow Puzzle
**Theme**: "Direct energy streams through sleight of hand gestures"

### Concept
Manipulate power redirectors to guide energy beams to targets. Click and drag redirectors to change their angles and redirect the power flow.

### Mechanics
- **Energy beams** shoot from power sources
- **Click and drag** redirectors to change their angle
- **Beam reflection**: Beams bounce off redirectors
- **Target hitting**: Beams must hit all required targets
- **Real-time updates**: Beam paths update as you move redirectors

### Components
- **PowerFlowPuzzle** - Main puzzle controller
- **PowerSource** - Emits energy beams
- **PowerTarget** - Must be hit by beams
- **PowerRedirector** - Redirects beam direction
- **PowerBeam** - Visual beam representation

### Advanced Features
- **Multiple bounces**: Beams can bounce multiple times
- **Colored beams**: Different power sources emit different colors
- **Beam splitting**: Some redirectors can split beams
- **Moving elements**: Some components move over time

---

## Additional Puzzle Ideas

### 4. Power Rune Puzzle
**Theme**: "Trace mystical power runes with precise gestures"

Draw specific patterns on a surface by clicking and dragging. The player must recreate magical runes by tracing the correct shapes within time limits.

- **Gesture recognition**: Match drawn lines to target patterns
- **Precision scoring**: Accuracy affects puzzle completion
- **Multiple runes**: Chain together complex sequences
- **Power levels**: Different runes require different skill levels

### 5. Energy Orb Juggling
**Theme**: "Manipulate multiple power orbs simultaneously"

Keep multiple energy orbs in motion by clicking them before they fall. More orbs are added over time, requiring quick reflexes and strategic timing.

- **Click timing**: Click orbs to keep them airborne
- **Escalating difficulty**: More orbs added progressively
- **Chain reactions**: Orbs can interact with each other
- **Power combinations**: Matching orb colors creates bonuses

### 6. Power Lock Puzzle
**Theme**: "Crack the combination through careful observation"

A series of rotating power discs that must be aligned correctly. Click and drag to rotate each disc, watching for visual and audio cues that indicate correct alignment.

- **Disc rotation**: Mouse drag to rotate each disc
- **Visual feedback**: Glowing when correctly aligned
- **Audio cues**: Different tones for each disc
- **Multi-layer locks**: Multiple rings of discs to align

---

## Integration with Sleight Game

### Power Rewards
Completing puzzles grants power to the sleight:
- **Circuit Puzzle**: +50 power (electrical mastery)
- **Memory Puzzle**: +75 power (mental focus)
- **Flow Puzzle**: +100 power (energy manipulation)

### Difficulty Scaling
Puzzles become more complex as the sleight gains power:
- **More nodes/elements**: Additional components to manage
- **Shorter time limits**: Increased pressure
- **Complex patterns**: More intricate solutions required
- **Multiple objectives**: Several goals to achieve simultaneously

### Visual Consistency
All puzzles share the sleight power theme:
- **Energy effects**: Glowing, pulsing, flowing animations
- **Color coding**: Power levels indicated by color intensity
- **Sound design**: Magical/technological audio cues
- **UI style**: Consistent with main game interface

---

## Setup Instructions

### 1. Power Circuit Puzzle Setup
1. Create **PowerCircuitPuzzle** GameObject
2. Add **PowerNode** GameObjects with different types
3. Configure **connection range** and **required connections**
4. Set up **LineRenderer** prefab for connections
5. Assign **audio clips** and **particle effects**

### 2. Memory Pattern Puzzle Setup
1. Create **MemoryPatternPuzzle** GameObject
2. Add **PatternElement** GameObjects in circular layout
3. Set **pattern length** and **time limits**
4. Configure **audio clips** for each element type
5. Setup **UI elements** for start/replay buttons

### 3. Power Flow Puzzle Setup
1. Create **PowerFlowPuzzle** GameObject
2. Add **PowerSource** GameObjects with beam directions
3. Place **PowerTarget** GameObjects at goal positions
4. Add **PowerRedirector** GameObjects as mirrors/prisms
5. Configure **LineRenderer** prefab for beams

### Testing & Balancing
- **Difficulty curves**: Start easy, gradually increase complexity
- **Timing balance**: Ensure reasonable time limits
- **Accessibility**: Clear visual and audio feedback
- **Error recovery**: Allow players to retry without harsh penalties
- **Progression**: Unlock new puzzle types as sleight gains power

These puzzles provide engaging, skill-based challenges that complement the sleight's power progression while maintaining the magical theme of channeling and manipulating energy through precise gestures and timing.
