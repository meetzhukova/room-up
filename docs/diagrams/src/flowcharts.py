"""
Generates the Room Up flowcharts as SVG + PNG.

Layout is placed by hand (coordinates below), so every arrow is an
orthogonal line with sharp corners and nothing overlaps.

Usage:
    python3 flowcharts.py            # writes ../<name>.svg and ../<name>.png
"""

from pathlib import Path

import cairosvg

OUT_DIR = Path(__file__).resolve().parent.parent

FONT = "DejaVu Sans Mono, JetBrains Mono, Menlo, monospace"
BG = "#F4F4F4"
GRID = "#E7E7E7"
LINE = "#2B2B2B"

STYLES = {
    "term":     {"fill": "#1F1F1F", "text": "#FFFFFF"},
    "action":   {"fill": "#7B9EF0", "text": "#14213D"},
    "decision": {"fill": "#F5D757", "text": "#1F1F1F"},
    "danger":   {"fill": "#E06666", "text": "#FFFFFF"},
}


class Chart:
    def __init__(self, name, title, width, height):
        self.name = name
        self.title = title
        self.width = width
        self.height = height
        self.parts = []

    # ---------- shapes ----------
    def node(self, cx, cy, text, kind="action", w=220, h=52):
        style = STYLES[kind]
        x, y = cx - w / 2, cy - h / 2
        if kind == "decision":
            points = f"{cx},{y} {cx + w / 2},{cy} {cx},{y + h} {x},{cy}"
            self.parts.append(f'<polygon points="{points}" fill="{style["fill"]}"/>')
        else:
            radius = 12 if kind == "term" else 0
            self.parts.append(
                f'<rect x="{x}" y="{y}" width="{w}" height="{h}" rx="{radius}" fill="{style["fill"]}"/>'
            )
        self._text(cx, cy, text, style["text"], 14)

    def _text(self, cx, cy, text, color, size, anchor="middle", weight="normal"):
        lines = text.split("\n")
        line_height = size * 1.25
        start = cy - (len(lines) - 1) * line_height / 2
        for i, line in enumerate(lines):
            self.parts.append(
                f'<text x="{cx}" y="{start + i * line_height}" fill="{color}" font-family="{FONT}" '
                f'font-size="{size}" font-weight="{weight}" text-anchor="{anchor}" '
                f'dominant-baseline="central">{line}</text>'
            )

    # ---------- connectors ----------
    def edge(self, points, label=None, label_at=None, arrow=True, anchor="middle"):
        path = " ".join(f"{x},{y}" for x, y in points)
        marker = ' marker-end="url(#arrow)"' if arrow else ""
        self.parts.append(
            f'<polyline points="{path}" fill="none" stroke="{LINE}" stroke-width="1.6" '
            f'stroke-linejoin="miter"{marker}/>'
        )
        if label:
            lx, ly = label_at
            self._label(lx, ly, label, anchor)

    def junction(self, x, y):
        self.parts.append(f'<circle cx="{x}" cy="{y}" r="3.5" fill="{LINE}"/>')

    def _label(self, x, y, text, anchor):
        width = len(text) * 8.4 + 10
        if anchor == "middle":
            rx = x - width / 2
        elif anchor == "start":
            rx = x - 5
        else:
            rx = x - width + 5
        self.parts.append(f'<rect x="{rx}" y="{y - 10}" width="{width}" height="20" fill="{BG}"/>')
        self._text(x, y, text, LINE, 13, anchor=anchor)

    # ---------- page ----------
    def legend(self, y, items):
        x = 40
        self._text(x, y, "Legend:", "#555555", 12, anchor="start")
        x += 80
        for kind, text in items:
            style = STYLES[kind]
            if kind == "decision":
                self.parts.append(
                    f'<polygon points="{x + 14},{y - 9} {x + 28},{y} {x + 14},{y + 9} {x},{y}" '
                    f'fill="{style["fill"]}"/>'
                )
            else:
                radius = 4 if kind == "term" else 0
                self.parts.append(
                    f'<rect x="{x}" y="{y - 8}" width="28" height="16" rx="{radius}" fill="{style["fill"]}"/>'
                )
            self._text(x + 38, y, text, "#555555", 12, anchor="start")
            x += 38 + len(text) * 7.3 + 30

    def save(self):
        grid = (
            '<defs>'
            f'<pattern id="grid" width="20" height="20" patternUnits="userSpaceOnUse">'
            f'<path d="M 20 0 L 0 0 0 20" fill="none" stroke="{GRID}" stroke-width="1"/></pattern>'
            f'<marker id="arrow" viewBox="0 0 10 10" refX="9" refY="5" markerWidth="8" markerHeight="8" '
            f'orient="auto-start-reverse"><path d="M 0 0 L 10 5 L 0 10 z" fill="{LINE}"/></marker>'
            '</defs>'
        )
        header = []
        header.append(f'<rect width="{self.width}" height="{self.height}" fill="{BG}"/>')
        header.append(f'<rect width="{self.width}" height="{self.height}" fill="url(#grid)"/>')
        svg = (
            f'<svg xmlns="http://www.w3.org/2000/svg" width="{self.width}" height="{self.height}" '
            f'viewBox="0 0 {self.width} {self.height}">{grid}{"".join(header)}'
        )
        title_parts = []
        title_chart = Chart("", "", 0, 0)
        title_chart._text(40, 44, self.title, "#1F1F1F", 24, anchor="start", weight="bold")
        title_chart._text(40, 74, "Room Up · Flowchart", "#777777", 13, anchor="start")
        title_parts = title_chart.parts
        svg += "".join(title_parts) + "".join(self.parts) + "</svg>"

        svg_path = OUT_DIR / f"{self.name}.svg"
        png_path = OUT_DIR / f"{self.name}.png"
        svg_path.write_text(svg, encoding="utf-8")
        cairosvg.svg2png(bytestring=svg.encode("utf-8"), write_to=str(png_path), scale=2)
        print(f"wrote {svg_path.name}, {png_path.name}")


def navigation():
    c = Chart("navigation", "Screen Navigation", 1000, 900)

    c.node(500, 140, "App launch", "term")
    c.node(500, 230, "Load save data")
    c.node(500, 340, "Room View", w=220, h=60)
    c.node(170, 500, "Edit Mode", w=200)
    c.node(500, 500, "Shop", w=200)
    c.node(830, 500, "Mode Select", w=200)
    c.node(170, 660, "Inventory", w=200)
    c.node(830, 660, "Puzzle Round", w=200)
    c.node(830, 800, "Round Results", w=200)

    c.edge([(500, 166), (500, 204)])
    c.edge([(500, 256), (500, 310)])

    # Room <-> Edit
    c.edge([(390, 325), (130, 325), (130, 474)], "Edit", (115, 400), anchor="end")
    c.edge([(210, 474), (210, 355), (390, 355)], "Done", (225, 400), anchor="start")
    # Room <-> Shop
    c.edge([(470, 370), (470, 474)], "Shop", (455, 422), anchor="end")
    c.edge([(530, 474), (530, 370)], "Back", (545, 422), anchor="start")
    # Room <-> Mode
    c.edge([(610, 325), (870, 325), (870, 474)], "Play", (885, 400), anchor="start")
    c.edge([(790, 474), (790, 355), (610, 355)], "Back", (775, 400), anchor="end")
    # Edit <-> Inventory
    c.edge([(130, 526), (130, 634)], "Inventory", (115, 580), anchor="end")
    c.edge([(210, 634), (210, 526)], "Pick item / Close", (225, 580), anchor="start")
    # Mode -> Puzzle
    c.edge([(830, 526), (830, 634)], "Classic", (845, 580), anchor="start")
    # Puzzle <-> Results
    c.edge([(790, 686), (790, 774)], "Board full", (775, 730), anchor="end")
    c.edge([(870, 774), (870, 686)], "Play again", (885, 730), anchor="start")
    # Results -> Room
    c.edge([(730, 800), (650, 800), (650, 420), (580, 420), (580, 370)], "Home", (665, 760), anchor="start")

    c.legend(870, [("term", "Start"), ("action", "Screen")])
    c._text(960, 870, "Arrow label = button", "#555555", 12, anchor="end")
    c.save()


def one_move():
    c = Chart("one-move", "Puzzle — One Move", 1000, 1440)

    c.node(500, 140, "Wait for input", "term")
    c.node(500, 230, "Player presses a cell")
    c.node(500, 340, "Is there\nan item?", "decision", w=220, h=110)
    c.node(500, 460, "Start dotted line")
    c.node(500, 590, "Finger moved\nor released?", "decision", w=240, h=120)
    c.node(200, 740, "Next cell empty\nor same type?", "decision", w=250, h=120)
    c.node(200, 880, "Extend line", w=200)
    c.node(500, 760, "Released on\nsame type?", "decision", w=240, h=120)
    c.node(760, 760, "Remove line", w=180)
    c.node(500, 890, "Match pair")
    c.node(500, 1030, "Uncaptured\nsame-type\nneighbours?", "decision", w=260, h=140)
    c.node(200, 1030, "Capture\nneighbours", w=200, h=60)
    c.node(500, 1160, "Remove matched\nand captured items", w=240, h=60)
    c.node(500, 1260, "Add coins")
    c.node(500, 1360, "Back to input", "term")

    # main spine
    c.edge([(500, 166), (500, 204)])
    c.edge([(500, 256), (500, 285)])
    c.edge([(500, 395), (500, 434)], "Yes", (515, 414), anchor="start")
    c.edge([(500, 486), (500, 530)])

    # No item -> back to input
    c.edge([(610, 340), (920, 340), (920, 1360), (610, 1360)], "No", (630, 324), anchor="start")

    # Moved -> check next cell
    c.edge([(380, 590), (200, 590), (200, 680)], "Moved", (360, 574), anchor="end")
    c.edge([(200, 800), (200, 854)], "Yes", (215, 827), anchor="start")
    # return loop (Extend line / No) back into the spine above the "moved or released" check
    c.edge([(100, 880), (40, 880), (40, 510), (492, 510)])
    c.edge([(75, 740), (40, 740)], "No", (58, 724), arrow=False)
    c.junction(40, 740)

    # Released -> same type?
    c.edge([(500, 650), (500, 700)], "Released", (515, 675), anchor="start")
    # Not same type -> remove line -> back to input
    c.edge([(620, 760), (670, 760)], "No", (645, 744))
    c.edge([(760, 786), (760, 1360)], arrow=False)
    c.junction(760, 1360)
    # Same type -> match
    c.edge([(500, 820), (500, 864)], "Yes", (515, 842), anchor="start")
    c.edge([(500, 916), (500, 960)])

    # Group capture loop
    c.edge([(370, 1030), (300, 1030)], "Yes", (335, 1014))
    c.edge([(200, 1000), (200, 940), (492, 940)])
    c.edge([(500, 1100), (500, 1130)], "No", (515, 1115), anchor="start")

    c.edge([(500, 1190), (500, 1234)])
    c.edge([(500, 1286), (500, 1334)])

    c.legend(1410, [("term", "Start / End"), ("action", "Action"), ("decision", "Decision")])
    c.save()


def spawn_tick():
    c = Chart("spawn-tick", "Puzzle — Spawn Tick", 1000, 960)

    c.node(450, 140, "Spawn timer fires", "term")
    c.node(450, 260, "Is there an\nempty cell?", "decision", w=240, h=120)
    c.node(150, 260, "End round", "danger", w=200)
    c.node(150, 380, "Show Round\nResults", "term", w=200, h=60)
    c.node(450, 420, "Helper spawn\nroll (30%)\nsucceeded?", "decision", w=260, h=140)
    c.node(760, 560, "Stuck item with\nan empty\nneighbour?", "decision", w=260, h=140)
    c.node(450, 720, "Spawn random type\nin random empty cell", w=240, h=60)
    c.node(760, 720, "Spawn same type\nnext to stuck item", w=240, h=60)
    c.node(605, 850, "Restart spawn timer", "term", w=240)

    c.edge([(450, 166), (450, 200)])
    c.edge([(330, 260), (250, 260)], "No", (290, 244))
    c.edge([(150, 286), (150, 350)])
    c.edge([(450, 320), (450, 350)], "Yes", (465, 335), anchor="start")

    c.edge([(580, 420), (760, 420), (760, 490)], "Yes", (600, 404), anchor="start")
    c.edge([(450, 490), (450, 690)], "No", (465, 520), anchor="start")
    c.edge([(630, 560), (450, 560)], "No", (610, 544), anchor="end", arrow=False)
    c.junction(450, 560)
    c.edge([(760, 630), (760, 690)], "Yes", (775, 660), anchor="start")

    c.edge([(450, 750), (450, 850), (485, 850)])
    c.edge([(760, 750), (760, 850), (725, 850)])

    c.legend(930, [("term", "Start / End"), ("action", "Action"), ("decision", "Decision"), ("danger", "Ends round")])
    c.save()


if __name__ == "__main__":
    navigation()
    one_move()
    spawn_tick()
