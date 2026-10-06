# Contributing

This document describes how work is done in **Room Up**: branches, commits, pull requests, code style and releases.
It is written for the author and for anyone who joins the project later.

---

## 1. Workflow Overview

```
Issue → branch → commits → pull request → review → merge into main → release (end of milestone)
```

1. Every piece of work starts as an **issue** in a **milestone**.
2. Work happens in a **branch** created from `main`.
3. The branch is merged through a **pull request** that closes the issue.
4. When all issues of a milestone are closed, a **release** is tagged.

`main` must always open in Unity and run without errors.

## 2. Branches

Format: `<milestone>/<short-description>` — lowercase, words separated by `-`.

| Branch | Used for |
|---|---|
| `main` | Stable code. No direct commits |
| `v0.0/design-docs` | Pre-production documents |
| `v0.1/board-grid` | A feature or task in milestone v0.1 |
| `v0.2/fix-capture-bug` | A bug fix in milestone v0.2 |

One branch = one topic. Delete the branch after merging.

## 3. Commits

Commits follow [Conventional Commits](https://www.conventionalcommits.org/):

```
<type>(<scope>): <short summary in present tense>

<optional body: what and why, not how>
```

| Type | When |
|---|---|
| `feat` | New gameplay feature or behaviour |
| `fix` | Bug fix |
| `docs` | Documentation only |
| `art` | Sprites, animations, other visual assets |
| `refactor` | Code change without new behaviour |
| `test` | Tests |
| `chore` | Project setup, settings, tooling |

**Scopes:** `puzzle`, `room`, `shop`, `ui`, `save`, `build`, `docs`.

Examples:
```
feat(puzzle): add dotted line that follows the finger
fix(puzzle): prevent line from passing through items
docs: add class diagram for the puzzle
chore: initialize Unity project
```

Rules:
- Summary up to ~60 characters, no period at the end.
- One logical change per commit.
- Always commit `.meta` files together with their assets.

## 4. Pull Requests

- Title in Conventional Commits style: `feat(puzzle): board grid and items`.
- Fill in the PR template.
- Link the issue with `Closes #12`, so it closes on merge.
- Add screenshots or a GIF for anything visual.
- Merge with **Squash and merge**, so `main` has one clean commit per PR.

**Definition of Done** — a PR can be merged when:
- [ ] The project opens in Unity without errors or new warnings
- [ ] The feature works in the Device Simulator
- [ ] The code follows the style below
- [ ] Docs are updated if behaviour changed (GDD, diagrams)
- [ ] `CHANGELOG.md` is updated under **Unreleased**

## 5. C# Code Style

Based on the Microsoft C# conventions and common Unity practice.

| Element | Style | Example |
|---|---|---|
| Classes, structs, enums | PascalCase | `PuzzleRound`, `ItemType` |
| Methods | PascalCase | `TryExtend()`, `AddCoins()` |
| Public properties | PascalCase | `Coins` |
| Private fields | camelCase | `roundCoins`, `spawnTimer` |
| Parameters, local variables | camelCase | `cell`, `deltaTime` |
| Constants | PascalCase | `MaxItemTypes` |
| Interfaces | `I` + PascalCase | `ISaveable` |

- Braces on a new line (Allman style).
- One class per file; file name = class name.
- Fields are `private` by default. Use `[SerializeField] private` to show a field in the Unity Inspector.
- Methods that may fail start with `Try` and return `bool`.
- Methods that answer a question start with `Is`, `Has` or `Can` and return `bool`.
- Comments explain **why**, not what.

```csharp
public class Wallet
{
    private int coins;

    public int GetCoins()
    {
        return coins;
    }

    public bool TrySpend(int amount)
    {
        if (amount > coins)
        {
            return false;
        }

        coins -= amount;
        return true;
    }
}
```

## 6. Unity Rules

- Close scenes before committing and avoid editing the same scene in two branches.
- Asset folders: `Assets/Scripts`, `Assets/Scenes`, `Assets/Art`, `Assets/Prefabs`, `Assets/Settings`.
- Gameplay logic lives in plain C# classes; `MonoBehaviour` scripts only connect them to Unity (input, drawing).

## 7. Releases

- Versions follow [Semantic Versioning](https://semver.org/): `v0.1.0`, `v0.2.0` … `v1.0.0`.
- When a milestone is complete:
  1. Move the **Unreleased** section of `CHANGELOG.md` under the new version.
  2. Create a tag and a GitHub Release with the changelog notes.
  3. From v0.2 on, attach an Android build (`.apk`) to the release.
