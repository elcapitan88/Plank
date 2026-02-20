# PLANK — Pirate Social Deduction Game
## Context & Instructions for Claude Code

---

## WHO I AM

I'm Cruz. I'm a solo developer building a multiplayer social deduction game called **PLANK** as a side project. My background is in full-stack web dev (FastAPI, React, Python, JavaScript) and I run an algorithmic trading SaaS called AtomikTrading. I'm strong on backend/systems but **new to Unity and C# game development**. I'm working out of VS Code with Claude Code connected.

---

## WHAT WE'RE BUILDING

**PLANK** is a multiplayer social deduction game (like Among Us) with a pirate theme, built in **Unity (C#)** for **PC/Steam**.

### Core Concept
- 4-10 players on a pirate ship
- Most players are **Crew** doing tasks to keep the ship sailing
- 1-2 players are **Mutineers** secretly killing crew and sabotaging the ship
- When a body is found or the ship's bell is rung, everyone discusses and votes to make someone walk the plank
- Crew wins by completing all tasks or ejecting all mutineers
- Mutineer wins by killing enough crew or completing a critical sabotage

### Tech Stack
- **Engine:** Unity (2D, top-down perspective)
- **Language:** C#
- **Networking:** Photon PUN 2
- **Version Control:** Git + GitHub
- **Platform:** PC (Steam)
- **IDE:** VS Code with C# Dev Kit + Unity extensions

---

## HOW TO WORK WITH ME

- I'm new to Unity and C#. **Explain what things do and why** as you write code. Don't just drop scripts — teach me as we go.
- Walk me through things **one step at a time**. Don't dump 5 tasks on me at once.
- When I need to do something in the Unity Editor (visual stuff you can't do), give me **exact step-by-step instructions** with menu paths like: `GameObject → 2D Object → Sprite → Square`.
- When writing C# scripts, include comments explaining what each section does.
- If I hit an error, help me debug it. I'll paste the error message.
- **Ask me to confirm each step is done before moving to the next one.**
- Keep track of where we are in the phase checklist below.

---

## DEVELOPMENT PHASES — WALK ME THROUGH THESE IN ORDER

### ✅ PHASE 0: Setup (Do This First)

#### Step 0.1 — Unity Installation
Walk me through:
- Installing Unity Hub
- Installing the correct Unity LTS version
- Creating a Unity Personal account
- Creating a new 2D project called "Plank"

#### Step 0.2 — VS Code Configuration
Walk me through:
- Confirming VS Code extensions are installed (C# Dev Kit, Unity, IntelliCode for C# Dev Kit)
- Setting VS Code as Unity's external script editor (Edit → Preferences → External Tools)
- Verifying double-clicking a script in Unity opens VS Code

#### Step 0.3 — Git Setup
Help me:
- Initialize git in the Unity project folder
- Verify the .gitignore covers Unity-specific files (Library/, Temp/, Logs/, obj/, etc.)
- Create a GitHub repo
- Push the initial commit

#### Step 0.4 — Photon Setup
Walk me through:
- Creating a Photon account at photonengine.com
- Creating a new Photon PUN 2 app in the dashboard
- Importing Photon PUN 2 Free from Unity Asset Store into the project
- Entering the App ID in Unity (PUN Wizard)

#### Step 0.5 — Verify Everything Works
Help me:
- Create a simple scene with a colored square
- Write a basic WASD movement script (PlayerController.cs)
- Attach it to the square and test in Unity Play mode
- Confirm the square moves with keyboard input
- Commit and push to GitHub

**✅ Phase 0 is done when: I can move a square around in Unity and the project is on GitHub.**

---

### 🔲 PHASE 1: Multiplayer Foundation (Weeks 1-3)

#### Step 1.1 — Lobby System
Build:
- A lobby scene with UI: player name input, create room button, join room button, room list
- LobbyManager.cs — handles connecting to Photon, creating/joining rooms
- Display connected players in the lobby
- A "Start Game" button (only visible to host)

#### Step 1.2 — Networked Player Spawning
Build:
- A game scene that loads when the host starts the game
- Player prefab with a sprite, collider, and PhotonView
- PlayerController.cs — WASD movement, but only for the local player (don't move other people's characters)
- Spawn each player when they enter the game scene
- Sync player positions across the network

#### Step 1.3 — Player Identity
Build:
- Assign each player a unique color on join
- Display player name above their character
- Camera follows the local player only

#### Step 1.4 — Basic Map
Build:
- A simple rectangular ship layout using Unity Tilemap or box colliders
- Walls that block player movement
- A few interior rooms/corridors
- At least 2 doorways connecting rooms

#### Step 1.5 — Test Multiplayer
Help me:
- Build and run two instances of the game
- Connect both to the same room
- Verify both players see each other moving
- Fix any sync issues

**✅ Phase 1 is done when: Two players can connect to a lobby, join a game, and move around a basic map seeing each other in real-time.**

---

### 🔲 PHASE 2: Core Game Loop (Weeks 4-6)

#### Step 2.1 — Game State Manager
Build:
- GameManager.cs — master script that controls the game state
- Game states: Lobby → Playing → Discussion → Voting → Results → Game Over
- State transitions synced across all clients

#### Step 2.2 — Role Assignment
Build:
- On game start, GameManager assigns Crew/Mutineer roles
- Only the local player sees their own role
- UI indicator showing your role (Crew or Mutineer)
- Mutineer count scales with player count

#### Step 2.3 — Kill System
Build:
- Mutineer sees a "Kill" button when near a crew member
- Killing creates a body object at the death location
- Killed player enters spectator mode
- Kill has a cooldown timer (25 seconds)
- Kill is synced via Photon RPCs

#### Step 2.4 — Body Reporting
Build:
- Crew sees a "Report" button when near a body
- Reporting triggers discussion phase for all players
- Show where the body was found

#### Step 2.5 — Emergency Meeting
Build:
- Place a Ship's Bell interactable on the map
- Any living player can ring it
- Cooldown per player (60 seconds)
- Triggers discussion phase

#### Step 2.6 — Discussion & Chat
Build:
- Discussion overlay screen showing all living players
- Text chat (send/receive messages via Photon)
- Discussion timer (configurable, default 30 seconds)

#### Step 2.7 — Voting System
Build:
- After discussion, show voting UI
- Click a player to vote for them, or click "Skip"
- Tally votes on the master client
- Announce result (ejected or skipped)
- Reveal ejected player's role
- Ejected player enters spectator mode

#### Step 2.8 — Win/Loss Detection
Build:
- After each kill: check if mutineers >= crew (mutineer wins)
- After each vote: check if all mutineers are ejected (crew wins)
- Win/loss screen with full role reveal
- "Return to Lobby" button

**✅ Phase 2 is done when: A full round can be played from start to finish — roles, kills, reports, discussion, voting, and a winner declared. PLAYTEST WITH FRIENDS HERE.**

---

### 🔲 PHASE 3: Tasks & Sabotage (Weeks 7-9)

#### Step 3.1 — Task Framework
Build:
- TaskManager.cs — assigns 5-6 random tasks to each crew member at game start
- Task interaction points placed on the map (visual indicators)
- Task list UI (checklist on left side of screen)
- Global task progress bar (visible to all players)
- Crew wins when task bar reaches 100%

#### Step 3.2 — Task Mini-Games (build 8-10)
Build each as a popup overlay when player interacts with a task point:
1. **Steer the Ship** — Rotate wheel to match target angle (slider or drag)
2. **Hoist the Sails** — Click and drag rope upward to a target
3. **Load Cannons** — Drag 3 cannonballs into the cannon in order
4. **Swab the Deck** — Move mop left/right across highlighted area
5. **Patch the Hull** — Drag wood pieces over holes (simple puzzle)
6. **Sort the Cargo** — Drag crates to matching colored zones
7. **Cook the Stew** — Click ingredients in displayed order
8. **Tie Knots** — Trace a pattern with mouse
9. **Light Lanterns** — Click lanterns in sequence (Simon Says style)
10. **Read the Map** — Click the correct grid coordinate

#### Step 3.3 — Sabotage System
Build:
- Mutineer gets a "Sabotage" button (cooldown 30 seconds)
- **Flood the Hull** — Countdown timer. Multiple crew must interact with hull to stop it. If timer runs out → mutineer wins.
- **Lights Out** — Vision radius shrinks dramatically below deck. Crew must interact with lantern points to fix.
- **Lock Doors** — A section of the ship is blocked for 10-15 seconds.
- Sabotage alerts shown to all players
- Sabotages disabled during discussion/voting

**✅ Phase 3 is done when: Crew can do tasks, progress bar fills, mutineer can sabotage. Game has genuine strategy now. PLAYTEST AGAIN.**

---

### 🔲 PHASE 4: Full Ship Map & Vision (Weeks 10-12)

#### Step 4.1 — Three-Deck Ship Map
Build the full ship layout:
- **Upper Deck**: Helm, Crow's Nest, Mast, Ship's Bell, Cannons, The Plank
- **Main Deck**: Galley, Captain's Quarters, Armory, Crew Quarters
- **Lower Deck**: Cargo Hold, Brig, Hull, Powder Magazine
- Ladders/stairs connecting decks
- Distribute task points and sabotage points across all decks

#### Step 4.2 — Vision / Fog of War
Build:
- Players can only see a limited radius around them
- Walls block line of sight
- Other players and bodies only visible within vision range
- Lower deck has smaller vision radius (darker)

#### Step 4.3 — Mutineer Tunnels
Build:
- 2-3 hidden tunnel entrances only visible to mutineers
- Connect distant parts of the ship
- Short travel animation (2-3 seconds)

#### Step 4.4 — Map Polish
- Room name labels
- Minimap showing layout (no player positions)
- Test map flow and fix dead ends or exploitable spots

**✅ Phase 4 is done when: The full ship is built, vision creates tension, and the map feels like a real space.**

---

### 🔲 PHASE 5: Art Pass (Weeks 13-16)
- Replace all placeholder art with purchased/commissioned assets
- Stylized cartoon pirate characters (8-10 color variants)
- Pirate ship tileset/environment art
- UI redesign (menus, HUD, voting screen, task UIs)
- Particle effects (water, fire, kill VFX, task completion)
- Walk the plank ejection sequence
- Game logo

**I'll likely handle art shopping/commissioning outside of Claude Code. Help me integrate assets when I bring them in.**

---

### 🔲 PHASE 6: Audio Pass (Weeks 16-18)
- Background music (lobby, gameplay, discussion, win/loss)
- Sound effects (footsteps, kill, report, bell, vote, plank splash, tasks, sabotage)
- Audio manager script with volume controls
- Help me implement the audio system and hook up all the sound triggers

---

### 🔲 PHASE 7: Polish & QA (Weeks 19-21)
- Bug fixing from playtests
- Edge case handling (disconnects, host migration, mid-game leaves)
- Tutorial / how to play screen
- Settings menu (graphics, audio, keybinds)
- Accessibility (colorblind mode, text size)
- Performance optimization

---

### 🔲 PHASE 8: Steam Integration & Launch (Weeks 22-24)
- Steamworks.NET integration
- Steam lobbies + friend invites
- Steam achievements
- Store page (screenshots, trailer, description)
- Beta testing
- Launch

---

### 🔲 PHASE 9: Post-Launch
- Bug fixes
- Content updates (new tasks, cosmetics, sabotages)
- Future: new maps, special roles, voice chat

---

## PROJECT STRUCTURE REFERENCE

Expected Unity project structure (build this as we go):

```
Plank/
├── Assets/
│   ├── Scenes/
│   │   ├── MainMenu.unity
│   │   ├── Lobby.unity
│   │   └── Game.unity
│   ├── Scripts/
│   │   ├── Networking/
│   │   │   ├── LobbyManager.cs
│   │   │   ├── PhotonManager.cs
│   │   │   └── RoomManager.cs
│   │   ├── Player/
│   │   │   ├── PlayerController.cs
│   │   │   ├── PlayerRole.cs
│   │   │   └── PlayerInteraction.cs
│   │   ├── GameFlow/
│   │   │   ├── GameManager.cs
│   │   │   ├── GameState.cs
│   │   │   ├── VotingManager.cs
│   │   │   └── WinConditionChecker.cs
│   │   ├── Tasks/
│   │   │   ├── TaskManager.cs
│   │   │   ├── TaskBase.cs (abstract)
│   │   │   ├── SteerShipTask.cs
│   │   │   ├── HoistSailsTask.cs
│   │   │   └── ... (one per task)
│   │   ├── Sabotage/
│   │   │   ├── SabotageManager.cs
│   │   │   ├── FloodSabotage.cs
│   │   │   ├── LightsOutSabotage.cs
│   │   │   └── LockDoorsSabotage.cs
│   │   ├── UI/
│   │   │   ├── HUDManager.cs
│   │   │   ├── VotingUI.cs
│   │   │   ├── ChatUI.cs
│   │   │   ├── TaskListUI.cs
│   │   │   └── MenuUI.cs
│   │   └── Audio/
│   │       └── AudioManager.cs
│   ├── Prefabs/
│   │   ├── Player.prefab
│   │   ├── Body.prefab
│   │   └── TaskPoints/
│   ├── Art/
│   │   ├── Characters/
│   │   ├── Environment/
│   │   ├── UI/
│   │   └── VFX/
│   ├── Audio/
│   │   ├── Music/
│   │   └── SFX/
│   ├── Tilemaps/
│   └── Photon/
│       └── PhotonServerSettings.asset
├── Packages/
├── ProjectSettings/
└── .gitignore
```

---

## CURRENT STATUS

**Phase: 0 — Setup**
**Current Step: 0.1 — Unity Installation**
**Last Completed: N/A — Just starting**

Update this section as we progress so we always know where we are.

---

## IMPORTANT REMINDERS

- I'm working on this 15-20 hours per week as a side project
- I'm solo — no team, no artist (buying assets when needed)
- My main business is AtomikTrading — don't let this take over
- I learn best by doing, not just reading. Get me building ASAP.
- Explain C# concepts as they come up — I know Python, not C#
- If something needs to be done in the Unity Editor, give me click-by-click instructions
- Commit to git frequently
- If I ask "what's next?" just check the phase checklist and give me the next step
