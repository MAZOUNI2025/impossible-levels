from pathlib import Path
from PIL import Image, ImageEnhance, ImageFilter, ImageDraw

root = Path('/home/ubuntu/IMPOSSIBLE_LEVELS')
reference = Image.open(root / 'visual_reference.png').convert('RGB')
reference = reference.resize((768, 768), Image.Resampling.LANCZOS)

palette = [
    ((10, 18, 52), (255, 172, 28), (112, 58, 255)),
    ((8, 24, 61), (255, 197, 67), (22, 205, 198)),
    ((16, 15, 55), (241, 150, 35), (154, 70, 255)),
    ((7, 29, 58), (255, 181, 31), (25, 171, 201)),
]

for level in range(11, 31):
    variant = (level - 11) % len(palette)
    dark, amber, accent = palette[variant]
    angle = ((level * 13) % 21) - 10
    image = reference.rotate(angle, resample=Image.Resampling.BICUBIC, expand=False)
    image = ImageEnhance.Color(image).enhance(0.72 + ((level % 4) * 0.08))
    image = ImageEnhance.Contrast(image).enhance(0.92 + ((level % 3) * 0.06))
    overlay = Image.new('RGBA', image.size, (*dark, 72))
    image = Image.alpha_composite(image.convert('RGBA'), overlay)
    draw = ImageDraw.Draw(image, 'RGBA')
    # Add a simple deterministic puzzle motif unique to each level.
    cx, cy = 384, 384
    radius = 90 + ((level * 17) % 170)
    for ring in range(1, 4):
        r = radius + ring * 34
        draw.ellipse((cx-r, cy-r, cx+r, cy+r), outline=(*accent, 85 - ring * 14), width=5)
    for i in range(2 + level % 4):
        x = 110 + ((level * 83 + i * 151) % 548)
        y = 120 + ((level * 47 + i * 97) % 520)
        size = 28 + ((level + i) % 3) * 16
        draw.rounded_rectangle((x-size, y-size, x+size, y+size), radius=10, fill=(*dark, 170), outline=(*amber, 180), width=5)
    draw.ellipse((cx-28, cy-28, cx+28, cy+28), fill=(*amber, 230), outline=(*accent, 220), width=5)
    draw.polygon([(cx, cy-18), (cx+18, cy), (cx, cy+18), (cx-18, cy)], fill=(*accent, 235))
    # Add a tiny stage marker in the corner; the number is only a development asset label.
    draw.rounded_rectangle((26, 26, 145, 84), radius=14, fill=(3, 8, 28, 210), outline=(*amber, 200), width=3)
    draw.text((52, 40), f'{level:02d}', fill=(255, 240, 188, 245))
    image.convert('RGB').save(root / f'level_{level:02d}.png', quality=94)
