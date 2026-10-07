"""
Generates the Room Up room class diagram (UML-style) as SVG + PNG.

Uses the drawing helpers from class_diagram.py.

Usage:
    python3 room_class_diagram.py    # writes ../room-class-diagram.svg and ../room-class-diagram.png
"""

import cairosvg

import class_diagram as cd


def main():
    width = 1560
    row1 = 130

    inv_x, inv_w = 40, 400
    inv_h = cd.uml_class(
        inv_x, row1, inv_w, "Inventory",
        ["- items: List<FurnitureData>"],
        [
            "+ GetItems(): IReadOnlyList<FurnitureData>",
            "+ Add(item: FurnitureData): void",
            "+ Remove(item: FurnitureData): bool",
        ],
    )

    rg_x, rg_w = 560, 500
    rg_h = cd.uml_class(
        rg_x, row1, rg_w, "RoomGrid",
        [
            "- size: int",
            "- wallHeight: int",
            "- cells: Dictionary<RoomSurface, PlacedFurniture[,]>",
            "- placed: List<PlacedFurniture>",
        ],
        [
            "+ RoomGrid(size, wallHeight: int)",
            "+ GetWidth(surface): int",
            "+ GetHeight(surface): int",
            "+ IsInside(surface, cell): bool",
            "+ GetAt(surface, cell): PlacedFurniture",
            "+ GetPlaced(): IReadOnlyList<PlacedFurniture>",
            "+ CanPlace(furniture): bool",
            "+ Place(furniture): bool",
            "+ Remove(furniture): void",
            "+ ClampOrigin(surface, origin, footprint): Vector2Int",
            "- SetCells(furniture, value): void",
        ],
    )

    iso_x, iso_w = 1140, 380
    iso_h = cd.uml_class(
        iso_x, row1, iso_w, "IsoProjection",
        [
            "- halfTileWidth: float",
            "- halfTileHeight: float",
            "- wallCellHeight: float",
        ],
        [
            "+ IsoProjection(tileW, tileH, wallH)",
            "+ ToWorld(roomPoint: Vector3): Vector2",
            "+ ToWorld(surface, point): Vector2",
            "+ CellCenter(surface, cell): Vector2",
            "+ CellAt(surface, world): Vector2Int",
            "+ ToRoomPoint(surface, point): Vector3",
        ],
    )

    row2 = row1 + max(inv_h, rg_h, iso_h) + 100

    fd_x, fd_w = 40, 400
    fd_h = cd.uml_class(
        fd_x, row2, fd_w, "FurnitureData",
        [
            "+ name: string",
            "+ size: Vector2Int",
            "+ isWallItem: bool",
            "+ height: float",
            "+ color: Color",
        ],
        [],
        stereotype="serializable",
    )

    pf_x, pf_w = 560, 500
    pf_h = cd.uml_class(
        pf_x, row2, pf_w, "PlacedFurniture",
        [
            "- data: FurnitureData",
            "- surface: RoomSurface",
            "- origin: Vector2Int",
            "- rotation: int",
        ],
        [
            "+ PlacedFurniture(data, surface, origin)",
            "+ GetData(): FurnitureData",
            "+ GetSurface(): RoomSurface",
            "+ GetOrigin(): Vector2Int",
            "+ GetRotation(): int",
            "+ GetSize(): Vector2Int",
            "+ MoveTo(surface, origin): void",
            "+ Rotate(): void",
            "+ GetCells(): List<Vector2Int>",
        ],
    )

    rs_x, rs_w = 1220, 220
    rs_h = cd.uml_class(
        rs_x, row2, rs_w, "RoomSurface",
        ["Floor", "LeftWall", "RightWall"],
        [],
        stereotype="enumeration",
    )

    cd.connector([(240, row1 + inv_h), (240, row2)],
                 start="diamond-hollow", label="0..*", label_at=(252, row2 - 14))
    cd.connector([(810, row1 + rg_h), (810, row2)],
                 start="diamond-hollow", label="0..*", label_at=(822, row2 - 14))
    cd.connector([(pf_x, row2 + 70), (fd_x + fd_w, row2 + 70)],
                 end="arrow-open", label="1", label_at=(fd_x + fd_w + 14, row2 + 56))
    cd.connector([(pf_x + pf_w, row2 + 70), (rs_x, row2 + 70)],
                 end="arrow-open", label="1", label_at=(rs_x - 22, row2 + 56))
    cd.connector([(1330, row1 + iso_h), (1330, row2)],
                 end="arrow-open", dashed=True, label="uses", label_at=(1342, (row1 + iso_h + row2) / 2))

    height = row2 + max(fd_h, pf_h, rs_h) + 130
    cd.legend(height - 70)
    cd.connector([(300, height - 42), (350, height - 42)], end="arrow-open")
    cd.text(362, height - 42, "refers to (stores a reference)", 12, color="#555555")

    body = list(cd.parts)
    cd.parts.clear()
    cd.text(40, 44, "Room — Class Diagram", 24, color="#1F1F1F", weight="bold")
    cd.text(40, 74, "Room Up · v0.3 Room Prototype", 13, color="#777777")
    title = list(cd.parts)

    svg = (
        f'<svg xmlns="http://www.w3.org/2000/svg" width="{width}" height="{height}" '
        f'viewBox="0 0 {width} {height}">{cd.markers()}'
        f'<rect width="{width}" height="{height}" fill="{cd.BG}"/>'
        f'<rect width="{width}" height="{height}" fill="url(#grid)"/>'
        + "".join(title) + "".join(body) + "</svg>"
    )

    (cd.OUT_DIR / "room-class-diagram.svg").write_text(svg, encoding="utf-8")
    cairosvg.svg2png(bytestring=svg.encode("utf-8"), write_to=str(cd.OUT_DIR / "room-class-diagram.png"), scale=2)
    print("wrote room-class-diagram.svg, room-class-diagram.png")


if __name__ == "__main__":
    main()
