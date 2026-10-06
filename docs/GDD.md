# Room Up — Game Design Document

> *Room Up* is a working title and may change before release.

| | |
|---|---|
| **Status** | Draft v0.3 |
| **Author** | Polina Zhukova |
| **Engine** | Unity (latest LTS), C# |
| **Platform** | Mobile: Android (v1.0), iOS (later) |
| **Controls** | Touch: tap and drag |
| **Orientation** | Portrait 9:16 |
| **Genre** | Casual puzzle + cozy room decoration |
| **Art style** | Bright, saturated isometric pixel art |
| **Inspiration** | *#OneRoom* (mobile, ~2018), cozy decoration games |

**Related documents:** [Flowcharts](flowcharts.md) · [Class Diagram](class-diagram.md)

---

## 1. Pitch

A cozy pixel-art game where you connect matching items with a line to earn coins and spend them on furniture to decorate your own rooms.

## 2. Setting & Player Fantasy

The player owns a house. At first only one room is open — the others are locked and wait to be unlocked later.
The fantasy: turning an empty room into *your* room, item by item.

## 3. Core Loop

```
Play a puzzle round → earn coins → buy furniture in the shop
→ place it in the room → room looks better → play again
```

## 4. Game Modes

| Mode | Purpose |
|---|---|
| **Room View** | Home screen. An isometric room seen as a "cube" with two walls. Player places, moves and rotates furniture. Buttons: Play, Shop, Inventory |
| **Puzzle** | A round of the connect-the-pairs puzzle. Earns coins |
| **Round Results** | Coins earned in the round, best score |
| **Shop** | Catalog of furniture to buy with coins |

Flow: `Main Menu → Room View ⇄ Puzzle → Round Results → Room View ⇄ Shop`

## 5. Puzzle

### 5.1 Board
- Grid of **8 columns × 10 rows**.
- Each cell is empty or holds one item.
- Top bar: Back button (left), coins earned in this round (right).

### 5.2 Items
- v1.0 uses **6 item types**, shown as simple colors (red, blue, green, yellow, purple, orange).
- At the start of a round **~40% of the cells** are filled with random items.

### 5.3 Connecting
- The player presses on an item and **drags a line** through **empty cells** (up, down, left, right — no diagonals).
- The line is shown as a **dotted line** while dragging.
- The line **cannot pass through other items**.
- If the player releases on an item of the **same type** → **match**: both items disappear.
- If the player releases anywhere else → the line disappears, nothing happens.

### 5.4 Group Capture (Combo)
- When a pair is matched, every item of the **same type** that touches either of the two items (up / down / left / right) is **captured** too, and disappears with them.
- Captured items keep spreading: a same-type item touching a captured one is also captured.
- Example: connect green → green, and a third green stands next to one of them → all three disappear.

### 5.5 Rewards
- **2 coins** for each matched item.
- **+3 coins** bonus for each captured item.

### 5.6 Spawning
- **One** new item appears every **6 seconds** in a random empty cell — slow on purpose, because the player needs time to think.
- The interval becomes shorter during the round (minimum 3 seconds).
- **Helper spawn:** sometimes (30% chance) the new item gets the same type as an item that currently cannot be connected to anything, and appears right next to it — so the player can match them at once.

### 5.7 End of Round
- The round ends when a new pair cannot fit: the **board is full**.
- The player always **keeps** the coins earned. No punishment — a short round just means fewer coins.

### 5.8 Difficulty (designed now, used later)
- Difficulty is controlled by the **number of item types**: more types = harder to find pairs = more coins.
- v1.0 has one mode: **Classic** (6 colors).
- Up to **5 harder modes** with more types and themed icons (flowers, kitchen…) are planned for later — see [Out of Scope](#11-out-of-scope-v10).

## 6. Room Decoration

### 6.1 Room
- Isometric room shown as a "cube": floor + left and right back walls.
- **Floor grid:** 6 × 6 cells.
- **Wall grids:** each of the two walls has its own grid, 6 cells wide × 4 cells high. Wall items snap to it.
- The grid is visible only while the player is placing or moving an item.

### 6.2 Furniture
- Each item has a **footprint** in cells: 1×1, 1×2 or 2×2.
- Items snap to the grid and **cannot overlap**. To put something where the bed stands, the player must first move the bed.
- Items can be **rotated** in 4 directions (0°, 90°, 180°, 270°).
- **Floor items** go on the floor. **Wall items** (pictures, posters, shelves) go on the two visible walls.
- Items can be moved again or put back into the inventory at any time.

### 6.3 Inventory
- Bought items go to the **inventory** until they are placed.

## 7. Shop & Economy

- The shop sells **8 furniture items** in v1.0.
- Prices: 20–150 coins.
- Target: one puzzle round ≈ 20–50 coins, so the first item can be bought after 1–2 rounds.
- All numbers will be tuned during playtesting.

## 8. Saving

Saved automatically on the device:
- coins
- inventory
- placed furniture (item, position, rotation)
- best round score

## 9. Screen & UI

- **Portrait 9:16**, made for phones.
- Pixel-perfect rendering: base resolution **360 × 640**, scaled ×2 / ×3 (×3 = 1080 × 1920).
- Taller phones get extra background space at the top and bottom; gameplay stays inside the safe 9:16 area (away from the camera notch).
- Day-to-day testing on the computer with Unity's **Device Simulator**; regular checks on a real Android phone.
- Buttons and items big enough for a finger; no actions that need hover or two fingers.
- All in-game text is in English.

## 10. Art & Audio

- **Style:** bright, juicy, colorful pixel art. Isometric "cube" room with skirting boards, simple shadows under furniture.
- **Doors and windows** are optional — they can be on the walls the player does not see.
- **Rotation art:** each furniture item needs sprites for its directions. To save time, 2 directions are drawn and the other 2 are mirrored.
- **Audio:** none in v1.0.

## 11. Out of Scope (v1.0)

Saved for later versions:
- Harder puzzle modes with more item types and themed icons (flowers, kitchen…), unlocked or bought, with bigger rewards
- Several rooms, a full house, buying new houses
- House styles and furniture styles (loft, modern, high-tech…)
- Wall paint and floor customization
- Rotating the room 360° to decorate all four walls
- House view (whole house visible, other rooms faded)
- Sound and music
- iOS build, desktop build

## 12. Roadmap

| Version | Milestone | Goal |
|---|---|---|
| v0.1 | Puzzle Prototype | Grid, items, drag a line, matching — with placeholder squares |
| v0.2 | Puzzle Complete | Group capture, coins, spawning, end of round, results |
| v0.3 | Room Prototype | Isometric grid, place / move / rotate furniture |
| v0.4 | Shop & Save | Shop, inventory, economy, saving |
| v0.5 | Art & UI | Pixel art, menus, polish |
| v1.0 | Release | Bug fixes, balance, Android build (APK on GitHub Releases / itch.io) |

## 13. Open Questions

- Final game name (working title: *Room Up*).
- Final numbers (spawn speed, start fill, coin rewards, prices).
