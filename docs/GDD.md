# Room Up — Game Design Document

> *Room Up* is a working title and may change before release.

| | |
|---|---|
| **Status** | v0.4 — updated after v0.2 Puzzle Complete |
| **Author** | Polina Zhukova |
| **Engine** | Unity 6, C# |
| **Platform** | Mobile: Android (v1.0), iOS (later) |
| **Controls** | Touch: tap (puzzle), tap and drag (room) |
| **Orientation** | Portrait 9:16 |
| **Genre** | Casual puzzle + cozy room decoration |
| **Art style** | Bright, saturated isometric pixel art |
| **Inspiration** | *#OneRoom* (mobile, ~2018), cozy decoration games |

**Related documents:** [Flowcharts](flowcharts.md) · [Class Diagram](class-diagram.md)

---

## 1. Pitch

A cozy pixel-art game where you tap empty cells to match items, earn coins and spend them on furniture to decorate your own rooms.

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

### 5.3 Tap to Match
- The player taps an **empty cell**.
- From that cell the game looks in **four directions** — up, down, left, right — and takes the **first item** in each direction. Items behind it do not count.
- Among these up to four items, every type that appears **two or more times** is matched: those items disappear.
- Two different pairs can match with one tap (e.g. 2 yellow + 2 blue).
- A short dotted link is drawn from the tapped cell to each matched item, then they disappear.
- Tapping an item, or an empty cell that sees no pair, does nothing.

```
  ·  ·  ·  🟨 ·  ·
  ·  ·  ·  ·  ·  ·
  🟨 ·  ·  ✕  ·  🟦      tap ✕ → 2 yellow + 2 blue disappear
  ·  ·  ·  ·  ·  ·
  ·  ·  ·  🟦 ·  ·
```

### 5.4 Combo
- **3 or 4 items** removed by one tap is a **combo**.
- A combo shows a short "Combo! +N" message.

### 5.5 Rewards
- **2 coins** for each removed item.
- Combo bonus: **+3 coins** for each item above two.

| Items removed | Coins |
|---|---|
| 2 | 4 |
| 3 (combo) | 9 |
| 4 (combo) | 14 |

- Coins earned in the round are shown at the top of the screen and also go to the player's wallet.

### 5.6 Spawning
- New items appear in random empty cells on a timer.
- The first interval is **3 s**; each spawn makes the next one **3% shorter**, down to **1 s**.
- **Low fill boost:** when less than **30%** of the board is filled, items appear faster — up to **4×** faster on an empty board.
- **Several at once:** each spawn has a **30%** chance to add one more item, up to **3** items at once.
- **Helper spawn:** **30%** of new items are placed so that an empty cell sees them and an existing item of the same type — a guaranteed possible match.
- **Cleared cells** cannot receive new items for **1.5 s**, so items never pop up right where the player just cleared.
- All numbers are tuned in `SpawnSettings` in the Inspector.

### 5.7 End of Round
- The round ends when a new item cannot fit: the **board is full**.
- The player always **keeps** the coins earned. No punishment — a short round just means fewer coins.
- The **Round Over** screen shows the coins of the round and the best score (saved on the device), with **Play Again** and **Home**.

### 5.8 Difficulty (designed now, used later)
- Difficulty is controlled by the **number of item types**: more types = harder to find pairs = more coins.
- v1.0 has one mode: **Classic** (6 colors).
- Up to **5 harder modes** with more types and themed icons (flowers, kitchen…) are planned for later — see [Out of Scope](#11-out-of-scope-v10).

## 6. Room Decoration

### 6.1 Room
- Isometric room shown as a "cube": floor + left and right back walls.
- **Floor grid:** 5 × 5 cells.
- **Wall grids:** each of the two walls has its own grid, 5 cells wide × 5 cells high. Wall items snap to it.
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
- The camera fits the board to the screen width on any phone (`CameraFitter`). Pixel-perfect rendering is planned for v0.5, when the final pixel art is added.
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
| v0.1 | Puzzle Prototype | Grid, items, matching — with placeholder squares ✅ |
| v0.2 | Puzzle Complete | Tap to match, combo, coins, spawning, end of round, results ✅ |
| v0.3 | Room Prototype | Isometric grid, place / move / rotate furniture |
| v0.4 | Shop & Save | Shop, inventory, economy, saving |
| v0.5 | Art & UI | Pixel art, menus, polish |
| v1.0 | Release | Bug fixes, balance, Android build (APK on GitHub Releases / itch.io) |

## 13. Open Questions

- Final game name (working title: *Room Up*).
- Final numbers (spawn speed, start fill, coin rewards, prices).
