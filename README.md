# Chill Farm

> A relaxing 2D farming RPG built with Unity — plant crops, raise animals, fish, and enjoy a peaceful rural life.

**[Download on itch.io](https://zussic.itch.io/chill-farm)**

![Unity](https://img.shields.io/badge/Unity-000000?style=for-the-badge&logo=unity&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)
![Firebase](https://img.shields.io/badge/Firebase-FFCA28?style=for-the-badge&logo=firebase&logoColor=black)
![Platform](https://img.shields.io/badge/Platform-PC-blue?style=for-the-badge)

![Gameplay](https://img.itch.zone/aW1hZ2UvNDIwNjM2NS8yNjE1ODIxNi5wbmc=/250x600/%2F8Ni1J.png)
![Screenshot](https://img.itch.zone/aW1hZ2UvNDIwNjM2NS8yNjE1ODIyNi5wbmc=/250x600/%2B%2F5B8H.png)
![Screenshot](https://img.itch.zone/aW1hZ2UvNDIwNjM2NS8yNjE1ODIzNS5wbmc=/250x600/Dd%2FDJH.png)

---

## Description

**Chill Farm** is a 2D top-down farming RPG inspired by Stardew Valley. Manage your own farm — plant and harvest crops, raise livestock, go fishing, complete quests, and interact with the local villagers. The game is built around a calm, satisfying loop with cozy pixel-art visuals.

---

## Features

### Farming
- Till soil, plant seeds, water crops, and harvest produces
- 5 crop types: Carrot, Cauliflower, Eggplant, Pumpkin, Tomato
- Fertilizer system to boost crop growth

### Livestock
- Raise Chickens and Cows across 3 quality grades
- Collect eggs and milk; breed animals to expand your herd
- Barn upgrade system for larger herds

### Fishing
- Interactive fishing mini-game
- 12+ fish species to catch across different spots

### Foraging
- Collect mushrooms across 4 varieties scattered around the world

### NPC & Dialogue
- 10+ NPCs with unique dialogue trees and patrol patterns
- Story-driven conversations with interaction system

### Quest System
- Full quest framework with multi-step objectives, tracking, and rewards
- In-game quest log UI

### Economy
- Shop for buying seeds, tools, and upgrades
- Sell crops, fish, and animal products at the shipping bin
- Gold management with in-game economy

### Progression
- Skill system and farm upgrades
- Day/time cycle to structure gameplay sessions
- Stamina system affecting player actions

### Cloud Save (Firebase)
- Account registration, login, and password recovery
- Save and load farm data to the cloud across sessions

---

## Tech Stack

| Category | Technology |
|---|---|
| Engine | Unity (URP) |
| Language | C# |
| Art Style | 2D Pixel Art |
| Rendering | Universal Render Pipeline + ShaderLab/HLSL |
| Input | Unity Input System |
| Camera | Cinemachine |
| Backend | Firebase (Auth + Firestore) |
| Version Control | Git |

---

## Getting Started

### Play
Download the latest build: [https://zussic.itch.io/chill-farm](https://zussic.itch.io/chill-farm)

### Run from Source

```bash
git clone https://github.com/Hoang-hai-yen/ChillFarm.git
```

1. Open **Unity Hub** → **Open Project** → select the cloned folder
2. Let Unity import all packages
3. Open `Assets/Scenes/Farm.unity`
4. Press **Play**

> **Note:** Cloud save features require a valid Firebase project configured in `Assets/Resources/apiConfig.json`.

---

## Project Structure

```
Assets/
├── Animations/     # Animation clips (player, NPCs, animals, UI)
├── Art/            # Sprites, tilesets, UI graphics, fonts
├── Audio/          # Music and sound effects
├── Data/           # ScriptableObject databases (items, quests, NPCs)
├── Materials/      # Shader materials
├── Prefabs/        # Reusable game object templates
├── Resources/      # Runtime-loaded assets and config
├── Scenes/         # Production scenes (Farm, Lobby, SignIn, ForgotPassword)
├── Scripts/        # All C# game logic (~140 files)
│   ├── Auth/       # Firebase authentication
│   ├── Cloud/      # Cloud save service layer
│   ├── Events/     # Decoupled event system
│   ├── Farming/    # Crop and farmland logic
│   ├── GameManager/# Core managers (inventory, time, weather, etc.)
│   ├── HUD/        # UI components
│   ├── Livestock/  # Animal system
│   ├── NPC/        # NPC dialogue and patrol
│   ├── QuestSystem/# Quest framework
│   └── Shop/       # Shop and sell logic
└── Settings/       # URP and project settings
```

---

## Team

| Name | GitHub |
|---|---|
| Hoang Hai Yen | [@Hoang-hai-yen](https://github.com/Hoang-hai-yen) |
| Dang Pham Nguyet Sang | [@Sanniverse](https://github.com/Sanniverse) |
| Vang Thanh Huy | [@vangthanhhuy125](https://github.com/vangthanhhuy125) |
| Pham Gia Huy | [@rhy221](https://github.com/rhy221) |

---

## License

MIT License — free to use for educational and non-commercial purposes.
