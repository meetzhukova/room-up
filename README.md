<div align="center">

# Room Up

**A cozy pixel-art mobile game: connect matching items to earn coins, then decorate your own rooms.**

![Status](https://img.shields.io/badge/status-pre--production-F5D757)
![Engine](https://img.shields.io/badge/engine-Unity-1F1F1F?logo=unity)
![Language](https://img.shields.io/badge/language-C%23-7B9EF0)
![Platform](https://img.shields.io/badge/platform-Android-3DDC84?logo=android&logoColor=white)

*Room Up is a working title and may change before release.*

</div>

---

## About

**Room Up** is a casual puzzle game with a cozy decoration meta-game, made for phones in portrait mode.

You earn coins in a puzzle round by drawing a dotted line between two matching items. Then you spend the coins on furniture and decorate an isometric pixel-art room — one item at a time.

The game is inspired by the mobile game *#OneRoom* (~2018), rebuilt from scratch with its own rules, art and progression.

## Core Loop

```
Play a puzzle round → earn coins → buy furniture → place it in the room → play again
```

## Planned Features (v1.0)

- **Connect puzzle** on an 8 × 10 board — drag a dotted line through empty cells to connect two items of the same type
- **Group Capture** — matching items that touch the connected pair are captured too, for bonus coins
- **Timed spawning** with a *helper spawn* that sometimes places a partner next to a stuck item
- **Isometric room** with floor and wall grids — place, move and rotate furniture
- **Shop and inventory** with 8 furniture items
- **Automatic saving** of coins, inventory and the room
- **Bright, saturated pixel art**, portrait 9:16

Full details: [Game Design Document](docs/GDD.md).

## Project Status

The project is in **pre-production**: design documents are done, development starts with milestone v0.1.

| Version | Milestone | Goal | Status |
|---|---|---|---|
| v0.0 | Pre-production | GDD, flowcharts, class diagram, repository setup | 🟡 In progress |
| v0.1 | Puzzle Prototype | Grid, items, drag a line, matching — placeholder squares | ⚪ Planned |
| v0.2 | Puzzle Complete | Group capture, coins, spawning, end of round, results | ⚪ Planned |
| v0.3 | Room Prototype | Isometric grid, place / move / rotate furniture | ⚪ Planned |
| v0.4 | Shop & Save | Shop, inventory, economy, saving | ⚪ Planned |
| v0.5 | Art & UI | Pixel art, menus, polish | ⚪ Planned |
| v1.0 | Release | Bug fixes, balance, Android build | ⚪ Planned |

Progress is tracked in [Milestones](https://github.com/meetzhukova/room-up/milestones) and [Issues](https://github.com/meetzhukova/room-up/issues).

## Documentation

| Document | Contents |
|---|---|
| [Game Design Document](docs/GDD.md) | Rules, content, screens, scope, roadmap |
| [Flowcharts](docs/flowcharts.md) | Screen navigation, one puzzle move, spawn tick |
| [Class Diagram](docs/class-diagram.md) | Classes of the puzzle and how they relate |

<p align="center">
  <img src="docs/diagrams/one-move.png" alt="Puzzle — One Move flowchart" width="420">
  &nbsp;&nbsp;
  <img src="docs/diagrams/class-diagram.png" alt="Puzzle class diagram" width="420">
</p>

## Tech Stack

| | |
|---|---|
| Engine | Unity (LTS) — exact version in `ProjectSettings/ProjectVersion.txt` |
| Language | C# |
| Target | Android (iOS later) |
| Editor | Visual Studio Code + Unity extension |
| Diagrams | Python (`docs/diagrams/src/`), Mermaid |

## Getting Started

### Requirements
- [Unity Hub](https://unity.com/download) with a Unity **LTS** editor
- Unity module **Android Build Support** (with OpenJDK and Android SDK & NDK Tools)
- Git

### Run the project
```bash
git clone https://github.com/meetzhukova/room-up.git
```
1. Open **Unity Hub → Add → Add project from disk** and select the `room-up` folder.
2. Open the project with the Unity version shown in Unity Hub.
3. Use **Window → General → Device Simulator** to test on phone screens.

> The Unity project is added in milestone v0.1.

## Project Structure

```
room-up/
├── Assets/            # Unity assets: scripts, scenes, sprites (from v0.1)
├── Packages/          # Unity package manifest
├── ProjectSettings/   # Unity project settings
├── docs/              # Design documents and diagrams
│   ├── GDD.md
│   ├── flowcharts.md
│   ├── class-diagram.md
│   └── diagrams/      # PNG / SVG images and the scripts that generate them
├── .github/           # Issue and pull request templates
├── CHANGELOG.md
└── CONTRIBUTING.md
```

## Development Workflow

- `main` is always stable. All work happens in branches and is merged through **pull requests**.
- Branches are named after the milestone: `v0.1/board-grid`, `v0.2/group-capture`.
- Commits follow [Conventional Commits](https://www.conventionalcommits.org/): `feat:`, `fix:`, `docs:`, `chore:`…
- Versions follow [Semantic Versioning](https://semver.org/); every milestone ends with a tagged release.

Details: [CONTRIBUTING.md](CONTRIBUTING.md) · History: [CHANGELOG.md](CHANGELOG.md)

## License

© 2026 Polina Zhukova. All rights reserved.
The code and art in this repository may not be copied, modified or distributed without permission.

## Author

**Polina Zhukova** — game design, art and development
[GitHub](https://github.com/meetzhukova)
