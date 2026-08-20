from pathlib import Path
import json

root = Path('/home/ubuntu/IMPOSSIBLE_LEVELS')
checks = []

def check(name, passed, detail):
    checks.append({'name': name, 'passed': bool(passed), 'detail': detail})

scripts = list((root / 'Assets/Scripts').rglob('*.cs'))
thumbs = sorted(root.glob('level_*.png'))
ui_icons = sorted(root.glob('ui_*.png'))
required = [
    root / 'FINAL_GAME_SPEC.md',
    root / 'BUILD_AND_PUBLISH.md',
    root / 'STORE_LISTING.md',
    root / 'PRIVACY_POLICY_TEMPLATE.md',
    root / 'PRODUCTION_MONETIZATION.md',
    root / 'FINAL_ASSET_REGISTER.md',
    root / 'Assets/Scripts/Levels/ProceduralPuzzleBoard.cs',
    root / 'Assets/Scripts/UI/HookIntroAndMotion.cs',
    root / 'Assets/Scripts/UI/PlayerProfilePanel.cs',
]

check('C# source files', len(scripts) >= 16, f'{len(scripts)} files found')
check('Level thumbnails', len(thumbs) == 30, f'{len(thumbs)} PNG files found')
check('UI icon set', len(ui_icons) >= 5, f'{len(ui_icons)} UI icons found')
check('Required release documents', all(p.exists() for p in required), f'{sum(p.exists() for p in required)}/{len(required)} present')
check('Progression hooks', 'CompleteLevel' in (root / 'Assets/Scripts/Levels/ProceduralPuzzleBoard.cs').read_text(), 'procedural board saves completion')
check('Touch input hooks', 'Input.touchCount' in (root / 'Assets/Scripts/Levels/ProceduralPuzzleBoard.cs').read_text(), 'Android touch path present')
check('Offline monetization safety', 'OfflineMonetizationGateway' in (root / 'Assets/Scripts/Monetization/MonetizationGateway.cs').read_text(), 'production adapters remain isolated')

result = {
    'project': 'IMPOSSIBLE LEVELS',
    'checks': checks,
    'passed': sum(c['passed'] for c in checks),
    'total': len(checks),
    'release_blockers': [
        'Unity Editor build and Android device test are required.',
        'AAB signing key and Play Console configuration are required.',
        'Live ad IDs and billing product IDs are not configured.',
        'Privacy policy placeholders must be replaced with the developer details and a public URL.'
    ]
}
(root / 'VALIDATION_REPORT.json').write_text(json.dumps(result, indent=2), encoding='utf-8')
print(json.dumps(result, indent=2))
