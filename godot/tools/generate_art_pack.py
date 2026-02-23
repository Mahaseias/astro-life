from PIL import Image, ImageDraw
from pathlib import Path
import random
import math

ROOT = Path('godot/AstroLifeGodot')

PALETTE = {
    'outline': (32, 45, 72, 255),
    'suit_light': (236, 241, 248, 255),
    'suit_mid': (193, 204, 221, 255),
    'suit_dark': (123, 139, 168, 255),
    'blue': (71, 157, 255, 255),
    'blue_dark': (41, 90, 160, 255),
    'amber_dark': (173, 109, 12, 255),
    'amber': (229, 154, 29, 255),
    'amber_light': (255, 205, 95, 255),
    'magenta': (226, 117, 204, 255),
    'bg_top': (11, 20, 66, 255),
    'bg_mid': (33, 40, 105, 255),
    'bg_low': (19, 103, 130, 255),
    'core_cyan': (71, 243, 255, 255),
    'core_cyan2': (32, 181, 255, 255),
    'boss_dark': (53, 47, 90, 255),
    'boss_mid': (91, 84, 130, 255),
    'boss_spike': (208, 70, 138, 255),
}


def ensure_dirs():
    for d in [
        ROOT / 'Art/Characters',
        ROOT / 'Art/Characters/Male',
        ROOT / 'Art/Characters/Female',
        ROOT / 'Art/Enemies',
        ROOT / 'Art/Backgrounds',
    ]:
        d.mkdir(parents=True, exist_ok=True)


def draw_block(draw, box, fill, outline=True):
    x0, y0, x1, y1 = box
    if outline:
        draw.rectangle((x0 - 1, y0 - 1, x1 + 1, y1 + 1), fill=PALETTE['outline'])
    draw.rectangle((x0, y0, x1, y1), fill=fill)


def draw_helmet(draw, img, cx, top, accent):
    outer = (cx - 12, top, cx + 12, top + 21)
    draw.ellipse((outer[0] - 1, outer[1] - 1, outer[2] + 1, outer[3] + 1), fill=PALETTE['outline'])
    draw.ellipse(outer, fill=PALETTE['suit_light'])
    draw.ellipse((cx - 9, top + 3, cx + 9, top + 17), fill=PALETTE['amber_dark'])

    px = img.load()
    for y in range(top + 3, top + 18):
        for x in range(cx - 9, cx + 10):
            rx = (x - cx) / 9.0
            ry = (y - (top + 10)) / 7.0
            if rx * rx + ry * ry <= 1.0:
                c = PALETTE['amber_light'] if y < top + 8 else (PALETTE['amber'] if y < top + 12 else PALETTE['amber_dark'])
                px[x, y] = c

    draw.ellipse((cx - 10, top + 4, cx - 5, top + 9), fill=(255, 244, 205, 255))
    draw.rectangle((cx - 13, top + 11, cx - 12, top + 16), fill=accent)


def draw_torso(draw, cx, top, accent):
    draw_block(draw, (cx - 10, top, cx + 10, top + 16), PALETTE['suit_light'])
    draw_block(draw, (cx - 13, top + 2, cx - 11, top + 15), PALETTE['suit_mid'])
    draw_block(draw, (cx + 11, top + 2, cx + 13, top + 15), PALETTE['suit_mid'])
    draw_block(draw, (cx - 6, top + 4, cx + 6, top + 10), PALETTE['suit_mid'])
    draw.rectangle((cx - 5, top + 5, cx + 5, top + 9), fill=PALETTE['suit_light'])
    draw.rectangle((cx - 2, top + 6, cx + 1, top + 8), fill=accent)
    draw.rectangle((cx - 9, top + 15, cx + 9, top + 16), fill=PALETTE['blue'])


def draw_backpack(draw, cx, top, accent):
    draw_block(draw, (cx + 11, top + 1, cx + 16, top + 15), PALETTE['suit_mid'])
    draw.rectangle((cx + 12, top + 4, cx + 15, top + 6), fill=accent)
    draw.rectangle((cx + 12, top + 8, cx + 15, top + 11), fill=PALETTE['blue'])


def draw_arm(draw, shoulder_x, shoulder_y, arm_shift, accent):
    draw_block(draw, (shoulder_x + arm_shift, shoulder_y, shoulder_x + arm_shift + 3, shoulder_y + 8), PALETTE['suit_light'])
    draw.rectangle((shoulder_x + arm_shift, shoulder_y + 2, shoulder_x + arm_shift + 1, shoulder_y + 5), fill=PALETTE['suit_mid'])
    draw_block(draw, (shoulder_x + arm_shift, shoulder_y + 9, shoulder_x + arm_shift + 2, shoulder_y + 11), accent)


def draw_leg(draw, hip_x, hip_y, leg_shift, foot_w, accent):
    draw_block(draw, (hip_x + leg_shift, hip_y, hip_x + leg_shift + 4, hip_y + 10), PALETTE['suit_light'])
    draw.rectangle((hip_x + leg_shift, hip_y + 6, hip_x + leg_shift + 4, hip_y + 7), fill=PALETTE['suit_mid'])
    draw_block(draw, (hip_x + leg_shift - 1, hip_y + 11, hip_x + leg_shift + foot_w, hip_y + 13), PALETTE['suit_mid'])
    draw.rectangle((hip_x + leg_shift, hip_y - 1, hip_x + leg_shift + 4, hip_y), fill=accent)


def draw_astronaut_frame(accent, pose):
    img = Image.new('RGBA', (64, 64), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)

    cx = 32 + pose.get('x', 0)
    base_y = 8 + pose.get('y', 0)

    draw_helmet(d, img, cx, base_y, accent)
    draw_torso(d, cx, base_y + 22, accent)
    draw_backpack(d, cx, base_y + 22, accent)
    draw_arm(d, cx - 16, base_y + 24, pose.get('arm_l', 0), accent)
    draw_arm(d, cx + 12, base_y + 24, pose.get('arm_r', 0), accent)
    draw_leg(d, cx - 7, base_y + 40, pose.get('leg_l', 0), 6, accent)
    draw_leg(d, cx + 2, base_y + 40, pose.get('leg_r', 0), 6, accent)

    d.line((cx - 2, base_y + 22, cx - 2, base_y + 39), fill=PALETTE['suit_dark'])
    d.line((cx + 2, base_y + 22, cx + 2, base_y + 39), fill=PALETTE['suit_dark'])
    d.rectangle((cx - 13, base_y + 32, cx - 11, base_y + 33), fill=PALETTE['blue'])
    d.rectangle((cx + 11, base_y + 32, cx + 13, base_y + 33), fill=PALETTE['blue'])

    return img


def save_character_pack(prefix, accent, sheet_path, fallback_path, portrait_path, frame_dir):
    poses = [
        {'y': 0, 'arm_l': 0, 'arm_r': 0, 'leg_l': 0, 'leg_r': 0},
        {'y': -1, 'arm_l': 1, 'arm_r': -1, 'leg_l': 0, 'leg_r': 0},
        {'y': 0, 'arm_l': 1, 'arm_r': -1, 'leg_l': -1, 'leg_r': 1},
        {'y': 0, 'arm_l': -1, 'arm_r': 1, 'leg_l': 1, 'leg_r': -1},
        {'y': -2, 'arm_l': -1, 'arm_r': 1, 'leg_l': 0, 'leg_r': 0},
        {'y': 1, 'arm_l': 1, 'arm_r': -1, 'leg_l': 0, 'leg_r': 0},
    ]
    frames = [draw_astronaut_frame(accent, p) for p in poses]

    sheet = Image.new('RGBA', (384, 64), (0, 0, 0, 0))
    for i, frame in enumerate(frames):
        sheet.paste(frame, (i * 64, 0), frame)
    sheet.save(sheet_path)
    frames[0].save(fallback_path)

    portrait = Image.new('RGBA', (128, 128), (13, 20, 56, 255))
    pdraw = ImageDraw.Draw(portrait)
    for r in range(60, 10, -6):
        c = (20 + r // 3, 30 + r // 4, 70 + r // 2, 255)
        pdraw.ellipse((64 - r, 64 - r, 64 + r, 64 + r), fill=c)
    large = frames[0].resize((112, 112), Image.Resampling.NEAREST)
    portrait.paste(large, (8, 8), large)
    pdraw.rectangle((12, 100, 116, 117), fill=(18, 35, 80, 220), outline=PALETTE['blue'])
    pdraw.text((18, 102), prefix.upper(), fill=(230, 238, 255, 255))
    portrait.save(portrait_path)

    frame_names = [
        f'{prefix.lower()}_idle_0.png',
        f'{prefix.lower()}_idle_1.png',
        f'{prefix.lower()}_walk_0.png',
        f'{prefix.lower()}_walk_1.png',
        f'{prefix.lower()}_jump_0.png',
        f'{prefix.lower()}_fall_0.png',
    ]
    for name, frame in zip(frame_names, frames):
        frame.save(frame_dir / name)


def draw_boss(body_path, core_path):
    img = Image.new('RGBA', (160, 96), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)
    cx, cy = 80, 48

    d.ellipse((48, 16, 112, 80), fill=PALETTE['boss_mid'], outline=PALETTE['outline'])
    d.ellipse((52, 20, 108, 76), fill=PALETTE['boss_dark'])

    for i in range(10):
        ang = (math.pi * 2 * i) / 10.0
        x = int(cx + math.cos(ang) * 34)
        y = int(cy + math.sin(ang) * 28)
        d.polygon([
            (x, y),
            (x + int(math.cos(ang + 0.35) * 10), y + int(math.sin(ang + 0.35) * 10)),
            (x + int(math.cos(ang - 0.35) * 10), y + int(math.sin(ang - 0.35) * 10)),
        ], fill=PALETTE['boss_spike'])

    for r, col in [(20, (40, 150, 255, 255)), (14, PALETTE['core_cyan2']), (9, PALETTE['core_cyan']), (4, (210, 255, 255, 255))]:
        d.ellipse((cx - r, cy - r, cx + r, cy + r), fill=col)

    for ax, ay in [(35, 30), (30, 62), (125, 30), (130, 62)]:
        d.line((cx, cy, ax, ay), fill=PALETTE['boss_mid'], width=5)
        d.ellipse((ax - 7, ay - 7, ax + 7, ay + 7), fill=PALETTE['boss_dark'], outline=PALETTE['outline'])
        d.polygon([(ax + 8, ay), (ax + 16, ay - 5), (ax + 13, ay + 2)], fill=(180, 190, 220, 255))
        d.polygon([(ax + 8, ay), (ax + 16, ay + 5), (ax + 13, ay - 2)], fill=(180, 190, 220, 255))

    for x in [60, 72, 88, 100]:
        d.rectangle((x, 74, x + 6, 90), fill=PALETTE['boss_dark'], outline=PALETTE['outline'])

    img.save(body_path)

    core = Image.new('RGBA', (48, 48), (0, 0, 0, 0))
    cd = ImageDraw.Draw(core)
    for r, col in [(22, (16, 80, 130, 180)), (16, PALETTE['core_cyan2']), (10, PALETTE['core_cyan']), (4, (235, 255, 255, 255))]:
        cd.ellipse((24 - r, 24 - r, 24 + r, 24 + r), fill=col)
    core.save(core_path)


def draw_background(path):
    w, h = 400, 225
    img = Image.new('RGBA', (w, h), PALETTE['bg_top'])
    px = img.load()

    for y in range(h):
        t = y / (h - 1)
        if t < 0.55:
            k = t / 0.55
            c0, c1 = PALETTE['bg_top'], PALETTE['bg_mid']
        else:
            k = (t - 0.55) / 0.45
            c0, c1 = PALETTE['bg_mid'], PALETTE['bg_low']
        r = int(c0[0] * (1 - k) + c1[0] * k)
        g = int(c0[1] * (1 - k) + c1[1] * k)
        b = int(c0[2] * (1 - k) + c1[2] * k)
        for x in range(w):
            px[x, y] = (r, g, b, 255)

    d = ImageDraw.Draw(img)

    random.seed(42)
    for _ in range(260):
        x = random.randint(0, w - 1)
        y = random.randint(0, h - 24)
        col = random.choice([(255, 226, 126, 255), (173, 221, 255, 255), (250, 250, 255, 255)])
        d.point((x, y), fill=col)
        if random.random() < 0.1 and x + 1 < w:
            d.point((x + 1, y), fill=col)

    for i in range(16):
        y = 118 + i * 3
        d.rectangle((0, y, w, y + 2), fill=(40 + i * 6, 30 + i * 3, 70 + i * 5, 90))

    d.ellipse((300, 12, 372, 84), fill=(78, 212, 245, 220))
    d.ellipse((318, 30, 354, 66), fill=(57, 145, 170, 210))
    d.ellipse((72, 30, 138, 96), fill=(80, 100, 200, 210))
    d.arc((58, 44, 150, 82), 10, 170, fill=(130, 170, 255, 220), width=2)

    d.rectangle((0, 175, w, 225), fill=(24, 30, 52, 255))
    for x in range(0, w, 28):
        d.rectangle((x, 178, x + 22, 191), fill=(32, 40, 68, 255), outline=(62, 82, 132, 255))
    for x in range(18, w, 64):
        d.rectangle((x, 194, x + 36, 198), fill=(76, 226, 240, 255))

    d.rectangle((0, 202, w, 225), fill=(38, 46, 78, 255))
    for y in range(206, 224, 4):
        d.line((0, y, w, y), fill=(64, 88, 136, 255))

    img.save(path)


def copy_img(src, dst):
    Image.open(src).save(dst)


def main():
    ensure_dirs()

    save_character_pack(
        'male', PALETTE['amber'],
        ROOT / 'Art/Characters/male_spritesheet_64x64.png',
        ROOT / 'Art/Characters/astronaut_male.png',
        ROOT / 'Art/Characters/astronaut_male_portrait.png',
        ROOT / 'Art/Characters/Male',
    )
    save_character_pack(
        'female', PALETTE['magenta'],
        ROOT / 'Art/Characters/female_spritesheet_64x64.png',
        ROOT / 'Art/Characters/astronaut_female.png',
        ROOT / 'Art/Characters/astronaut_female_portrait.png',
        ROOT / 'Art/Characters/Female',
    )

    copy_img(ROOT / 'Art/Characters/male_spritesheet_64x64.png', ROOT / 'Art/Characters/PlayerMale.png')
    copy_img(ROOT / 'Art/Characters/female_spritesheet_64x64.png', ROOT / 'Art/Characters/PlayerFemale.png')

    draw_boss(ROOT / 'Art/Enemies/collector_boss_body_160x96.png', ROOT / 'Art/Enemies/collector_boss_core_48.png')
    copy_img(ROOT / 'Art/Enemies/collector_boss_body_160x96.png', ROOT / 'Art/Enemies/CollectorBoss.png')
    copy_img(ROOT / 'Art/Enemies/collector_boss_core_48.png', ROOT / 'Art/Enemies/CollectorBossCore.png')

    draw_background(ROOT / 'Art/Backgrounds/space_bg.png')
    copy_img(ROOT / 'Art/Backgrounds/space_bg.png', ROOT / 'Art/Backgrounds/SpaceBg.png')

    print('Generated art pack successfully.')


if __name__ == '__main__':
    main()
