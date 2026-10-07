"""
Generates the Room Up shop & save class diagram (UML-style) as SVG + PNG.

Uses the drawing helpers from class_diagram.py.

Usage:
    python3 progress_class_diagram.py    # writes ../progress-class-diagram.png
"""

import cairosvg

import class_diagram as cd


def main():
    width = 1560
    row1 = 130

    lx, lw = 40, 420
    cx, cw = 580, 420
    rx, rw = 1120, 400

    cat_h = cd.uml_class(
        lx, row1, lw, "FurnitureCatalog",
        ["- items: List<FurnitureData>"],
        [
            "+ GetItems(): IReadOnlyList<FurnitureData>",
            "+ Find(itemName: string): FurnitureData",
        ],
        stereotype="ScriptableObject",
    )

    game_h = cd.uml_class(
        cx, row1, cw, "Game",
        [
            "+ RoomScene: string",
            "+ PuzzleScene: string",
            "- progress: PlayerProgress",
            "- catalog: FurnitureCatalog",
        ],
        [
            "+ Progress: PlayerProgress",
            "+ Catalog: FurnitureCatalog",
            "+ Save(): void",
            "+ OpenRoom(): void",
            "+ OpenPuzzle(): void",
        ],
        stereotype="static",
    )

    ss_h = cd.uml_class(
        rx, row1, rw, "SaveSystem",
        ["- FileName: string"],
        [
            "+ GetPath(): string",
            "+ Save(data: SaveData): void",
            "+ Load(): SaveData",
            "+ Delete(): void",
        ],
        stereotype="static",
    )

    row2 = row1 + max(cat_h, game_h, ss_h) + 100

    shop_h = cd.uml_class(
        lx, row2, lw, "Shop",
        ["- catalog: List<FurnitureData>"],
        [
            "+ Shop(items)",
            "+ GetItems(): IReadOnlyList<FurnitureData>",
            "+ CanBuy(item, wallet): bool",
            "+ TryBuy(item, wallet, inventory): bool",
        ],
    )

    pp_h = cd.uml_class(
        cx, row2, cw, "PlayerProgress",
        [
            "- wallet: Wallet",
            "- inventory: Inventory",
            "- placed: List<PlacedFurniture>",
            "- bestRoundCoins: int",
            "- isNew: bool",
        ],
        [
            "+ GetWallet(): Wallet",
            "+ GetInventory(): Inventory",
            "+ GetPlaced(): IReadOnlyList<...>",
            "+ SetPlaced(items): void",
            "+ GetBestRoundCoins(): int",
            "+ TrySetBestRoundCoins(coins): bool",
            "+ IsNew(): bool",
            "+ MarkStarted(): void",
            "+ ToSaveData(): SaveData",
            "+ FromSaveData(data, catalog)",
        ],
    )

    sd_h = cd.uml_class(
        rx, row2, rw, "SaveData",
        [
            "+ version: int",
            "+ coins: int",
            "+ bestRoundCoins: int",
            "+ inventory: List<string>",
            "+ placed: List<PlacedItemSave>",
        ],
        [],
        stereotype="serializable",
    )

    row3 = row2 + max(shop_h, pp_h, sd_h) + 100

    fd_h = cd.uml_class(
        lx, row3, lw, "FurnitureData",
        [
            "+ name: string",
            "+ size: Vector2Int",
            "+ isWallItem: bool",
            "+ height: float",
            "+ price: int",
            "+ color: Color",
        ],
        [],
        stereotype="serializable",
    )

    pis_h = cd.uml_class(
        rx, row3, rw, "PlacedItemSave",
        [
            "+ item: string",
            "+ surface: RoomSurface",
            "+ origin: Vector2Int",
            "+ rotation: int",
        ],
        [],
        stereotype="serializable",
    )

    cd.connector([(cx, row1 + 70), (lx + lw, row1 + 70)],
                 start="diamond-hollow", label="1", label_at=(lx + lw + 12, row1 + 56))
    cd.connector([(cx + cw, row1 + 70), (rx, row1 + 70)],
                 end="arrow-open", dashed=True, label="uses", label_at=(cx + cw + 40, row1 + 56))
    cd.connector([(790, row1 + game_h), (790, row2)],
                 start="diamond-filled", label="1", label_at=(802, row2 - 14))
    cd.connector([(1320, row1 + ss_h), (1320, row2)],
                 end="arrow-open", dashed=True, label="reads / writes",
                 label_at=(1332, (row1 + ss_h + row2) / 2))
    cd.connector([(cx + cw, row2 + 70), (rx, row2 + 70)],
                 end="arrow-open", dashed=True, label="converts", label_at=(cx + cw + 20, row2 + 56))
    cd.connector([(1320, row2 + sd_h), (1320, row3)],
                 start="diamond-filled", label="0..*", label_at=(1332, row3 - 14))
    cd.connector([(250, row2 + shop_h), (250, row3)],
                 start="diamond-hollow", label="0..*", label_at=(262, row3 - 14))
    cd.connector([(lx, row1 + 70), (18, row1 + 70), (18, row3 + 23), (lx, row3 + 23)],
                 start="diamond-filled", label="0..*", label_at=(lx + 6, row3 - 14))

    height = max(row3 + fd_h, row3 + pis_h) + 130
    cd.legend(height - 70)
    cd.connector([(300, height - 42), (350, height - 42)], end="arrow-open")
    cd.text(362, height - 42, "refers to (stores a reference)", 12, color="#555555")

    body = list(cd.parts)
    cd.parts.clear()
    cd.text(40, 44, "Shop & Save — Class Diagram", 24, color="#1F1F1F", weight="bold")
    cd.text(40, 74, "Room Up · v0.4 Shop & Save", 13, color="#777777")
    title = list(cd.parts)

    svg = (
        f'<svg xmlns="http://www.w3.org/2000/svg" width="{width}" height="{height}" '
        f'viewBox="0 0 {width} {height}">{cd.markers()}'
        f'<rect width="{width}" height="{height}" fill="{cd.BG}"/>'
        f'<rect width="{width}" height="{height}" fill="url(#grid)"/>'
        + "".join(title) + "".join(body) + "</svg>"
    )

    cairosvg.svg2png(bytestring=svg.encode("utf-8"), write_to=str(cd.OUT_DIR / "progress-class-diagram.png"), scale=2)
    print("wrote progress-class-diagram.png")


if __name__ == "__main__":
    main()
