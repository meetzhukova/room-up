# Room Up

A cozy pixel-art mobile game: match items to earn coins, then spend them on furniture to decorate your own rooms.

> *Room Up* is a working title.

## About

- **Puzzle:** tap an empty cell on an 8 × 10 board. The nearest items up, down, left and right are checked, and every pair of the same type disappears.
- **Decoration:** buy furniture with the coins and arrange it in an isometric room: a 5 × 5 floor and two walls.
- **Platform:** Android, portrait. Made with Unity and C#.

Full design: [Game Design Document](docs/GDD.md)

## Status

The full game loop works with placeholder graphics: play the puzzle, earn coins, buy furniture, decorate the room. Progress is saved. Next: v0.5 Art & UI.

| Version | Goal | Status |
|---|---|---|
| v0.0 Pre-production | Design documents, repository setup | Done |
| v0.1 Puzzle Prototype | Board, items, matching | Done |
| v0.2 Puzzle Complete | Tap to match, coins, spawning, results | Done |
| v0.3 Room Prototype | Room grid, place and rotate furniture | Done |
| v0.4 Shop & Save | Shop, inventory, saving | Done |
| v0.5 Art & UI | Pixel art, menus | Planned |
| v1.0 Release | Balance, bug fixes, Android build | Planned |

Tasks: [Milestones](https://github.com/sidequestion/room-up/milestones) · [Issues](https://github.com/sidequestion/room-up/issues)

## Documentation

- [Game Design Document](docs/GDD.md) — rules, content, screens, scope
- [Flowcharts](docs/flowcharts.md) — screen navigation and puzzle logic
- [Class Diagrams](docs/class-diagram.md) — puzzle, room, shop and save classes

## Getting Started

Requirements: Unity Hub, Unity 6 with Android Build Support, Git.

```bash
git clone https://github.com/sidequestion/room-up.git
```

Open the folder in Unity Hub (**Add → Add project from disk**). Scenes are in `Assets/Scenes`; start from `Room`, the home screen.

## Workflow

Work happens in branches named after the milestone (`v0.1/board-grid`) and is merged into `main` through pull requests. Commits follow [Conventional Commits](https://www.conventionalcommits.org/).

## License

© 2026 Polina Zhukova. All rights reserved.
