# Room Up

A cozy pixel-art mobile game: connect matching items to earn coins, then spend them on furniture to decorate your own rooms.

> *Room Up* is a working title.

## About

- **Puzzle:** drag a dotted line between two items of the same type on an 8 × 10 board. Matching items next to them are captured too.
- **Decoration:** buy furniture with the coins and arrange it in an isometric room.
- **Platform:** Android, portrait. Made with Unity and C#.

Full design: [Game Design Document](docs/GDD.md)

## Status

Pre-production — design is done, development starts with v0.1.

| Version | Goal | Status |
|---|---|---|
| v0.0 Pre-production | Design documents, repository setup | In progress |
| v0.1 Puzzle Prototype | Board, items, line, matching | Planned |
| v0.2 Puzzle Complete | Capture, coins, spawning, results | Planned |
| v0.3 Room Prototype | Room grid, place and rotate furniture | Planned |
| v0.4 Shop & Save | Shop, inventory, saving | Planned |
| v0.5 Art & UI | Pixel art, menus | Planned |
| v1.0 Release | Balance, bug fixes, Android build | Planned |

Tasks: [Milestones](https://github.com/meetzhukova/room-up/milestones) · [Issues](https://github.com/meetzhukova/room-up/issues)

## Documentation

- [Game Design Document](docs/GDD.md) — rules, content, screens, scope
- [Flowcharts](docs/flowcharts.md) — screen navigation and puzzle logic
- [Class Diagram](docs/class-diagram.md) — puzzle classes and their relations

## Getting Started

Requirements: Unity Hub, a Unity LTS editor with Android Build Support, Git.

```bash
git clone https://github.com/meetzhukova/room-up.git
```

Open the folder in Unity Hub (**Add → Add project from disk**). The Unity project is added in v0.1.

## Workflow

Work happens in branches named after the milestone (`v0.1/board-grid`) and is merged into `main` through pull requests. Commits follow [Conventional Commits](https://www.conventionalcommits.org/).

## License

© 2026 Polina Zhukova. All rights reserved.
