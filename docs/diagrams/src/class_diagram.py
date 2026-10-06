"""
Generates the Room Up puzzle class diagram (UML-style) as SVG + PNG.

Layout is placed by hand so every connector is orthogonal and nothing overlaps.

Usage:
    python3 class_diagram.py         # writes ../class-diagram.svg and ../class-diagram.png
"""

from pathlib import Path

import cairosvg

OUT_DIR = Path(__file__).resolve().parent.parent

FONT = "DejaVu Sans Mono, JetBrains Mono, Menlo, monospace"
BG = "#F4F4F4"
GRID = "#E7E7E7"
LINE = "#2B2B2B"
HEADER = "#7B9EF0"
HEADER_ENUM = "#F5D757"
BODY = "#FFFFFF"
BORDER = "#C9C9C9"
TEXT = "#14213D"

ROW = 22
HEADER_H = 46
PAD = 10

parts = []


def text(x, y, value, size=13, color=TEXT, anchor="start", weight="normal"):
    value = value.replace("&", "&amp;").replace("<", "&lt;").replace(">", "&gt;")
    parts.append(
        f'<text x="{x}" y="{y}" fill="{color}" font-family="{FONT}" font-size="{size}" '
        f'font-weight="{weight}" text-anchor="{anchor}" dominant-baseline="central">{value}</text>'
    )


def uml_class(x, y, w, name, fields, methods, stereotype=None):
    """Draws a class box and returns its height."""
    header_color = HEADER_ENUM if stereotype else HEADER
    fields_h = len(fields) * ROW + PAD * 2
    methods_h = len(methods) * ROW + PAD * 2 if methods else 0
    h = HEADER_H + fields_h + methods_h

    parts.append(f'<rect x="{x}" y="{y}" width="{w}" height="{h}" fill="{BODY}" stroke="{BORDER}"/>')
    parts.append(f'<rect x="{x}" y="{y}" width="{w}" height="{HEADER_H}" fill="{header_color}"/>')
    if stereotype:
        text(x + w / 2, y + 14, f"«{stereotype}»", 11, anchor="middle")
        text(x + w / 2, y + 31, name, 15, anchor="middle", weight="bold")
    else:
        text(x + w / 2, y + HEADER_H / 2, name, 15, anchor="middle", weight="bold")

    cy = y + HEADER_H + PAD + ROW / 2
    for f in fields:
        text(x + 12, cy, f)
        cy += ROW
    if methods:
        sep = y + HEADER_H + fields_h
        parts.append(f'<line x1="{x}" y1="{sep}" x2="{x + w}" y2="{sep}" stroke="{BORDER}"/>')
        cy = sep + PAD + ROW / 2
        for m in methods:
            text(x + 12, cy, m)
            cy += ROW
    return h


def connector(points, start=None, end=None, dashed=False, label=None, label_at=None, label_anchor="start"):
    path = " ".join(f"{px},{py}" for px, py in points)
    dash = ' stroke-dasharray="6 5"' if dashed else ""
    ms = f' marker-start="url(#{start})"' if start else ""
    me = f' marker-end="url(#{end})"' if end and not (dashed and end == "arrow-open") else ""
    parts.append(
        f'<polyline points="{path}" fill="none" stroke="{LINE}" stroke-width="1.6"{dash}{ms}{me}/>'
    )
    if dashed and end == "arrow-open":
        # cairosvg dashes marker strokes too, so draw a solid arrowhead by hand
        (x1, y1), (x2, y2) = points[-2], points[-1]
        dx, dy = (x2 > x1) - (x2 < x1), (y2 > y1) - (y2 < y1)
        px, py = -dy, dx
        a = (x2 - dx * 10 + px * 5, y2 - dy * 10 + py * 5)
        b = (x2 - dx * 10 - px * 5, y2 - dy * 10 - py * 5)
        parts.append(
            f'<polyline points="{a[0]},{a[1]} {x2},{y2} {b[0]},{b[1]}" fill="none" '
            f'stroke="{LINE}" stroke-width="1.6"/>'
        )
    if label:
        lx, ly = label_at
        text(lx, ly, label, 12, color=LINE, anchor=label_anchor)


def legend(y):
    x = 40
    text(x, y, "Legend:", 12, color="#555555")
    x += 80
    items = [
        ("composition", "owns (created and destroyed together)"),
        ("aggregation", "uses, but lives longer"),
        ("dependency", "uses as a method parameter"),
    ]
    for kind, label in items:
        if kind == "composition":
            connector([(x + 50, y), (x, y)], start=None, end="diamond-filled-end")
        elif kind == "aggregation":
            connector([(x + 50, y), (x, y)], end="diamond-hollow-end")
        else:
            connector([(x, y), (x + 50, y)], end="arrow-open", dashed=True)
        text(x + 62, y, label, 12, color="#555555")
        x += 62 + len(label) * 7.3 + 40
    text(40, y + 28, "+ public    - private", 12, color="#555555")


def markers():
    return (
        '<defs>'
        f'<pattern id="grid" width="20" height="20" patternUnits="userSpaceOnUse">'
        f'<path d="M 20 0 L 0 0 0 20" fill="none" stroke="{GRID}" stroke-width="1"/></pattern>'
        # diamonds drawn at the START of a path (owner side)
        f'<marker id="diamond-filled" viewBox="0 0 20 10" refX="20" refY="5" markerWidth="20" markerHeight="10" '
        f'orient="auto-start-reverse" markerUnits="userSpaceOnUse">'
        f'<path d="M 0 5 L 10 0 L 20 5 L 10 10 z" fill="{LINE}"/></marker>'
        f'<marker id="diamond-hollow" viewBox="0 0 20 10" refX="20" refY="5" markerWidth="20" markerHeight="10" '
        f'orient="auto-start-reverse" markerUnits="userSpaceOnUse">'
        f'<path d="M 1 5 L 10 0.8 L 19 5 L 10 9.2 z" fill="{BODY}" stroke="{LINE}" stroke-width="1.4"/></marker>'
        # same diamonds, for the END of a path (used in the legend)
        f'<marker id="diamond-filled-end" viewBox="0 0 20 10" refX="20" refY="5" markerWidth="20" markerHeight="10" '
        f'orient="auto" markerUnits="userSpaceOnUse">'
        f'<path d="M 0 5 L 10 0 L 20 5 L 10 10 z" fill="{LINE}"/></marker>'
        f'<marker id="diamond-hollow-end" viewBox="0 0 20 10" refX="20" refY="5" markerWidth="20" markerHeight="10" '
        f'orient="auto" markerUnits="userSpaceOnUse">'
        f'<path d="M 1 5 L 10 0.8 L 19 5 L 10 9.2 z" fill="{BODY}" stroke="{LINE}" stroke-width="1.4"/></marker>'
        f'<marker id="arrow-open" viewBox="0 0 12 12" refX="11" refY="6" markerWidth="12" markerHeight="12" '
        f'orient="auto" markerUnits="userSpaceOnUse">'
        f'<path d="M 1 1 L 11 6 L 1 11" fill="none" stroke="{LINE}" stroke-width="1.6" stroke-dasharray="none"/></marker>'
        '</defs>'
    )


def main():
    width = 1420

    # ---------- PuzzleRound (center top) ----------
    pr_x, pr_y, pr_w = 520, 130, 400
    pr_h = uml_class(
        pr_x, pr_y, pr_w, "PuzzleRound",
        [
            "- board: Board",
            "- line: Line",
            "- spawner: Spawner",
            "- wallet: Wallet",
            "- roundCoins: int",
            "- isOver: bool",
        ],
        [
            "+ PuzzleRound(wallet: Wallet)",
            "+ StartRound(): void",
            "+ Update(deltaTime: float): void",
            "+ OnPress(cell: Vector2Int): void",
            "+ OnDrag(cell: Vector2Int): void",
            "+ OnRelease(cell: Vector2Int): void",
            "+ GetRoundCoins(): int",
            "+ IsOver(): bool",
            "- TryMatch(a, b: Vector2Int): bool",
            "- CalculateReward(count: int): int",
            "- EndRound(): void",
        ],
    )

    # ---------- Wallet (right of PuzzleRound) ----------
    wa_x, wa_y, wa_w = 1060, 130, 320
    uml_class(
        wa_x, wa_y, wa_w, "Wallet",
        ["- coins: int"],
        [
            "+ GetCoins(): int",
            "+ AddCoins(amount: int): void",
            "+ TrySpend(amount: int): bool",
        ],
    )

    # ---------- second row ----------
    row_y = pr_y + pr_h + 90
    bo_x, bo_w = 40, 420
    bo_h = uml_class(
        bo_x, row_y, bo_w, "Board",
        [
            "- width: int",
            "- height: int",
            "- cells: Item[,]",
        ],
        [
            "+ IsInside(cell: Vector2Int): bool",
            "+ IsEmpty(cell: Vector2Int): bool",
            "+ GetItem(cell: Vector2Int): Item",
            "+ PlaceItem(item: Item, cell: Vector2Int): void",
            "+ RemoveItem(cell: Vector2Int): void",
            "+ HasEmptyCell(): bool",
            "+ GetEmptyCells(): List<Vector2Int>",
            "+ FindGroup(start: Vector2Int): List<Vector2Int>",
        ],
    )

    li_x, li_w = 510, 340
    li_h = uml_class(
        li_x, row_y, li_w, "Line",
        ["- cells: List<Vector2Int>"],
        [
            "+ Start(cell: Vector2Int): void",
            "+ TryExtend(cell: Vector2Int): bool",
            "+ GetStart(): Vector2Int",
            "+ GetEnd(): Vector2Int",
            "+ IsActive(): bool",
            "+ Clear(): void",
        ],
    )

    sp_x, sp_w = 900, 380
    sp_h = uml_class(
        sp_x, row_y, sp_w, "Spawner",
        [
            "- interval: float",
            "- minInterval: float",
            "- timer: float",
            "- helperChance: float",
        ],
        [
            "+ Tick(deltaTime: float): bool",
            "+ SpawnItem(board: Board): bool",
        ],
    )

    # ---------- third row ----------
    it_y = row_y + bo_h + 80
    it_x, it_w = 40, 340
    it_h = uml_class(
        it_x, it_y, it_w, "Item",
        ["- type: ItemType"],
        [
            "+ Item(type: ItemType)",
            "+ GetType(): ItemType",
            "+ IsSameType(other: Item): bool",
        ],
    )
    en_x, en_w = 470, 200
    en_h = uml_class(
        en_x, it_y, en_w, "ItemType",
        ["Red", "Blue", "Green", "Yellow", "Purple", "Orange"],
        [],
        stereotype="enumeration",
    )

    pr_bottom = pr_y + pr_h

    # ---------- connectors ----------
    # PuzzleRound ◆— Board
    connector([(560, pr_bottom), (560, pr_bottom + 40), (250, pr_bottom + 40), (250, row_y)],
              start="diamond-filled", label="1", label_at=(260, row_y - 14))
    # PuzzleRound ◆— Line
    connector([(680, pr_bottom), (680, row_y)],
              start="diamond-filled", label="1", label_at=(690, row_y - 14))
    # PuzzleRound ◆— Spawner
    connector([(880, pr_bottom), (880, pr_bottom + 40), (1090, pr_bottom + 40), (1090, row_y)],
              start="diamond-filled", label="1", label_at=(1100, row_y - 14))
    # PuzzleRound ◇— Wallet
    connector([(pr_x + pr_w, 200), (wa_x, 200)],
              start="diamond-hollow", label="1", label_at=(wa_x - 16, 186))

    # Board ◆— Item  (0..80 items: 8 x 10 cells)
    connector([(250, row_y + bo_h), (250, it_y)],
              start="diamond-filled", label="0..80", label_at=(262, it_y - 14))
    # Item —> ItemType
    connector([(it_x + it_w, it_y + 70), (en_x, it_y + 70)], end="arrow-open")
    # Spawner ..> Board (SpawnItem takes a Board)
    sp_bottom = row_y + sp_h
    dep_y = row_y + max(li_h, sp_h) + 40
    connector([(1090, sp_bottom), (1090, dep_y), (bo_x + bo_w, dep_y)],
              end="arrow-open", dashed=True, label="uses", label_at=(1100, dep_y - 16))

    height = it_y + max(it_h, en_h) + 130
    legend(height - 70)

    svg = (
        f'<svg xmlns="http://www.w3.org/2000/svg" width="{width}" height="{height}" '
        f'viewBox="0 0 {width} {height}">{markers()}'
        f'<rect width="{width}" height="{height}" fill="{BG}"/>'
        f'<rect width="{width}" height="{height}" fill="url(#grid)"/>'
    )
    title = []
    parts_backup = list(parts)
    parts.clear()
    text(40, 44, "Puzzle — Class Diagram", 24, color="#1F1F1F", weight="bold")
    text(40, 74, "Room Up · v0.1–v0.2 scope", 13, color="#777777")
    title = list(parts)
    parts.clear()
    parts.extend(parts_backup)

    svg += "".join(title) + "".join(parts) + "</svg>"

    (OUT_DIR / "class-diagram.svg").write_text(svg, encoding="utf-8")
    cairosvg.svg2png(bytestring=svg.encode("utf-8"), write_to=str(OUT_DIR / "class-diagram.png"), scale=2)
    print("wrote class-diagram.svg, class-diagram.png")


if __name__ == "__main__":
    main()
